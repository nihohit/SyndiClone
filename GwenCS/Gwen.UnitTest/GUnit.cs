using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
    public class GUnit : Base
    {
        public UnitTest UnitTest;

        public GUnit(Base parent) : base(parent)
        {
            
        }

        public void UnitPrint(String str)
        {
            if (UnitTest != null)
                UnitTest.PrintText(str);
        }
    }
}
