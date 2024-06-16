using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Server.Compression;
using Server.Gumps;
using Server.Network;

namespace Server.Tests
{
    public interface IGumpWriter
    {
        int TextEntries { get; set; }
        int Switches { get; set; }

        void AppendLayout(bool val);
        void AppendLayout(int val);
        void AppendLayout(uint val);
        void AppendLayout(Serial serial);
        void AppendLayoutNS(int val);
        void AppendLayout(string text);
        void AppendLayoutNS(string text);
        void AppendLayout(ReadOnlySpan<byte> buffer);
        void WriteStrings(List<string> strings);
        void Flush();
    }

    public sealed class CloseGump : Packet
    {
        public CloseGump(int typeID, int buttonID) : base(0xBF)
        {
            EnsureCapacity(13);

            Stream.Write((short)0x04);
            Stream.Write(typeID);
            Stream.Write(buttonID);
        }
    }

    public sealed class DisplayGumpPacked : Packet, IGumpWriter
    {
        private static readonly byte[] m_Buffer = new byte[48];

        private readonly Gump m_Gump;

        private readonly PacketWriter m_Layout;
        private readonly PacketWriter m_Strings;

        private int m_StringCount;

        static DisplayGumpPacked() => m_Buffer[0] = (byte)' ';

        public DisplayGumpPacked(Gump gump)
            : base(0xDD)
        {
            m_Gump = gump;

            m_Layout = PacketWriter.CreateInstance(8192);
            m_Strings = PacketWriter.CreateInstance(8192);
        }

        public int TextEntries { get; set; }

        public int Switches { get; set; }

        public void AppendLayout(bool val)
        {
            AppendLayout(val ? " 1"u8 : " 0"u8);
        }

        public void AppendLayout(int val)
        {
            var toString = val.ToString();
            var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1) + 1;

            m_Layout.Write(m_Buffer, 0, bytes);
        }

        public void AppendLayout(uint val)
        {
            var toString = val.ToString();
            var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1) + 1;

            m_Layout.Write(m_Buffer, 0, bytes);
        }

        public void AppendLayout(Serial serial) => AppendLayout(serial.Value);

        public void AppendLayoutNS(int val)
        {
            var toString = val.ToString();
            var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1);

            m_Layout.Write(m_Buffer, 1, bytes);
        }

        public void AppendLayoutNS(string text)
        {
            m_Layout.WriteAsciiFixed(text, text.Length);
        }

        public void AppendLayout(string text)
        {
            AppendLayout(" @"u8);

            m_Layout.WriteAsciiFixed(text, text.Length);

            AppendLayout("@"u8);
        }

        public void AppendLayout(ReadOnlySpan<byte> buffer)
        {
            m_Layout.Write(buffer);
        }

        public void WriteStrings(List<string> strings)
        {
            m_StringCount = strings.Count;

            for (var i = 0; i < strings.Count; ++i)
            {
                var v = strings[i] ?? "";

                m_Strings.Write((ushort)v.Length);
                m_Strings.WriteBigUniFixed(v, v.Length);
            }
        }

        public void Flush()
        {
            EnsureCapacity(28 + (int)m_Layout.Length + (int)m_Strings.Length);

            Stream.Write(m_Gump.Serial);
            Stream.Write(m_Gump.TypeID);
            Stream.Write(m_Gump.X);
            Stream.Write(m_Gump.Y);

            // Note: layout MUST be null terminated (don't listen to krrios)
            m_Layout.Write((byte)0);

            WritePacked(m_Layout);

            Stream.Write(m_StringCount);

            WritePacked(m_Strings);

            PacketWriter.ReleaseInstance(m_Layout);
            PacketWriter.ReleaseInstance(m_Strings);
        }

        private void WritePacked(PacketWriter src)
        {
            var buffer = src.UnderlyingStream.GetBuffer();
            var length = (int)src.Length;

            if (length == 0)
            {
                Stream.Write(0);
                return;
            }

            var wantLength = 1 + length * 1024 / 1000;

            wantLength += 4095;
            wantLength &= ~4095;

            var packBuffer = ArrayPool<byte>.Shared.Rent(wantLength);
            var bytesPacked = Deflate.Standard.Pack(packBuffer, buffer.AsSpan(0, length));

            Stream.Write(4 + bytesPacked);
            Stream.Write(length);
            Stream.Write(packBuffer, 0, bytesPacked);

            ArrayPool<byte>.Shared.Return(packBuffer);
        }
    }

    public sealed class DisplaySignGump : Packet
    {
        public DisplaySignGump(Serial serial, int gumpID, string unknown, string caption) : base(0x8B)
        {
            unknown ??= "";
            caption ??= "";

            EnsureCapacity(15 + unknown.Length + caption.Length);

            Stream.Write(serial);
            Stream.Write((short)gumpID);
            Stream.Write((short)(unknown.Length + 1));
            Stream.WriteAsciiNull(unknown);
            Stream.Write((short)(caption.Length + 1));
            Stream.WriteAsciiNull(caption);
        }
    }
}
