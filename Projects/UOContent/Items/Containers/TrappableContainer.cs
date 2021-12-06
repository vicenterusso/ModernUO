using Server.Network;

namespace Server.Items
{
    public enum TrapType
    {
        None,
        MagicTrap,
        ExplosionTrap,
        DartTrap,
        PoisonTrap
    }

    [Serializable(3, false)]
    public abstract partial class TrappableContainer : BaseContainer, ITelekinesisable
    {
        [SerializableField(0)]
        [SerializableFieldAttr("[CommandProperty(AccessLevel.GameMaster)]")]
        private int _trapLevel;

        [SerializableField(1)]
        [SerializableFieldAttr("[CommandProperty(AccessLevel.GameMaster)]")]
        private int _trapPower;

        [SerializableField(2)]
        [SerializableFieldAttr("[CommandProperty(AccessLevel.GameMaster)]")]
        private TrapType _trapType;

        public TrappableContainer(int itemID) : base(itemID)
        {
        }

        public virtual bool TrapOnOpen => true;

        public virtual void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(Location, Map, 0x1F5);

            if (TrapOnOpen)
            {
                ExecuteTrap(from);
            }
        }

        private void SendMessageTo(Mobile to, int number, int hue)
        {
            if (Deleted || !to.CanSee(this))
            {
                return;
            }

            to.NetState.SendMessageLocalized(Serial, ItemID, MessageType.Regular, hue, 3, number);
        }

        private void SendMessageTo(Mobile to, string text, int hue)
        {
            if (Deleted || !to.CanSee(this))
            {
                return;
            }

            to.NetState.SendMessage(Serial, ItemID, MessageType.Regular, hue, 3, false, "ENU", "", text);
        }

        public virtual bool ExecuteTrap(Mobile from)
        {
            if (_trapType == TrapType.None)
            {
                return false;
            }

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                SendMessageTo(from, "That is trapped, but you open it with your godly powers.", 0x3B2);
                return false;
            }

            SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

            var loc = GetWorldLocation();

            switch (_trapType)
            {
                case TrapType.ExplosionTrap:
                    {
                        ExecuteExplosionTrap(from, loc);
                        break;
                    }
                case TrapType.MagicTrap:
                    {
                        ExecuteMagicTrap(from, loc);
                        break;
                    }
                case TrapType.DartTrap:
                    {
                        ExecuteDartTrap(from, loc);
                        break;
                    }
                case TrapType.PoisonTrap:
                    {
                        ExecutePoisonTrap(from, loc);
                        break;
                    }
            }

            TrapType = TrapType.None;
            TrapPower = 0;
            TrapLevel = 0;
            return true;
        }

        public override void Open(Mobile from)
        {
            if (!TrapOnOpen || !ExecuteTrap(from))
            {
                base.Open(from);
            }
        }

        private void Deserialize(IGenericReader reader, int version)
        {
            base.Deserialize(reader);

            _trapLevel = reader.ReadInt();
            _trapPower = reader.ReadInt();
            _trapType = (TrapType)reader.ReadInt();
        }
    }
}
