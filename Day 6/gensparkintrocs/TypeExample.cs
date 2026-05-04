using System.ComponentModel;

namespace UnderstandingTypesApp
{
    internal class TypeExample
    {
        internal void ShowingConvertions()
        {
            int iNum1=100;
            float fNum2=123.4f;
            string strNum3 =null;
            fNum2=iNum1;//implict type casting
            System.Console.WriteLine($"The value of fNum2 is {fNum2}");
            iNum1 = (int)fNum2; //explict type casting
            iNum1 = Convert.ToInt32(Math.Round(fNum2));
            System.Console.WriteLine("ENter any number");
            iNum1 = Convert.ToInt32(Console.ReadLine());//unboxing
            strNum3 = iNum1.ToString(); //Boxing
            System.Console.WriteLine("The value of num1 is : " +iNum1);
        }
        internal void ShowingLimits()
        {
            int num1 = int.MaxValue;
            System.Console.WriteLine($"the value of num1 before is {num1}");
            num1++;
            System.Console.WriteLine($"the value of num1 after is {num1}");

            checked
            {
                 int num2 = int.MaxValue;
                System.Console.WriteLine($"the value of num1 before is {num2}");
                num2++;
                System.Console.WriteLine($"the value of num1 before is {num2}");

            }
        }
        internal void HandlingNulls()
        {
            int? num1 =null;
            int num2=100;
            int sum = num1??0 + num2;//if num1 is null use 0 there
            System.Console.WriteLine($"The sum is: {sum}");
            string strNum1 = null;
            num2 = Convert.ToInt32(strNum1);
            System.Console.WriteLine($"COnverted value of strNUm1 is {num2}");

        }
    }
}