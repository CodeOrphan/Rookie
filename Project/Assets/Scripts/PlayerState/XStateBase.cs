using System;
using System.Reflection;

namespace Game.Player
{
    public class ResetOnFrameBeginAttribute : Attribute
    {
        public ResetOnFrameBeginAttribute()
        {
        }
    }

    public class ResetOnFrameEndAttribute : Attribute
    {
        public ResetOnFrameEndAttribute()
        {
        }
    }

    public class OnStartOpenAttribute : Attribute
    {
        public OnStartOpenAttribute()
        {
        }
    }

    public static class StateAttributeMask
    {
        public static int BeginReset = 1;
        
        public static int EndReset = 1 << 1;
        
        public static int OnStartOpen = 1 << 2;
    }


    [Serializable]
    public class XPawnStateData
    {
        public string Name;
        
        public bool State;
        [NonSerialized] public int Mask;

    }

    [Serializable]
    public class XStateBase<TEnum> where TEnum : Enum
    {
        [NonSerialized]
        public XPawnStateData[] State;

        public Func<TEnum, int> CovertWeight;

        public XStateBase(Func<TEnum, int> covert)
        {
            CovertWeight = covert;
            Array values = Enum.GetValues(typeof(TEnum));
            if (values.Length <= 0)
            {
                return;
            }

            State = new XPawnStateData[(int) values.GetValue(values.Length - 1) + 1];

            foreach (var value in values)
            {
                var s = new XPawnStateData {Name = value.ToString()};

                if (GetAttribute<ResetOnFrameBeginAttribute>((TEnum) value))
                {
                    s.Mask |= StateAttributeMask.BeginReset;
                }

                if (GetAttribute<ResetOnFrameEndAttribute>((TEnum) value))
                {
                    s.Mask |= StateAttributeMask.EndReset;
                }

                if (GetAttribute<OnStartOpenAttribute>((TEnum) value))
                {
                    s.Mask |= StateAttributeMask.OnStartOpen;
                }

                // ReSharper disable once PossibleInvalidCastException
                State[((int) value)] = s;
            }

            Start();
        }

        public virtual bool GetState(TEnum stateDefine)
        {
            return State[CovertWeight(stateDefine)].State;
        }

        public virtual void SetState(TEnum stateDefine, bool state)
        {
            State[CovertWeight(stateDefine)].State = state;
        }

        private static bool GetAttribute<T>(Enum enumObj) where T : Attribute
        {
            Type type = enumObj.GetType();

            Attribute attr = null;
            try
            {
                String enumName = Enum.GetName(type, enumObj);
                FieldInfo field = type.GetField(enumName);
                attr = field.GetCustomAttribute(typeof(T), false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return attr != null;
        }

        public void Start()
        {
            foreach (var stateData in State)
            {
                if ((stateData.Mask & StateAttributeMask.OnStartOpen) != 0)
                {
                    stateData.State = true;
                }
            }
        }

        public void ResetOnBegin()
        {
            foreach (var stateData in State)
            {
                if ((stateData.Mask & StateAttributeMask.BeginReset) != 0)
                {
                    stateData.State = false;
                }
            }
        }

        public void ResetOnEnd()
        {
            foreach (var stateData in State)
            {
                if ((stateData.Mask & StateAttributeMask.EndReset) != 0)
                {
                    stateData.State = false;
                }
            }
        }
    }
}