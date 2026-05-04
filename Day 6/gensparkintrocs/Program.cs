namespace UnderstandingTypesApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // TypeExample typpeExample = new TypeExample();
            // typpeExample.ShowingConvertions();
            // typpeExample.ShowingLimits();
            // typpeExample.HandlingNulls();
            int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

            List<string> names = ["Alice", "Bob", "Charlie", "David"];
            names.Add("vaibhav"); // to push back new element
            names.Insert(1, "Ram"); // index 1 → value 15
            bool exists = names.Contains("vaibhav");
            int index = names.IndexOf("vaibhav");
            string second = names[1]; // 0-based index
            string last = names[^1]; // ^1 is the last element

            System.Console.WriteLine($"size of names: {names.Count} ");
            System.Console.WriteLine($"range slicing: names[2..4] ");

            foreach(var name in names[2..4])
            {
                System.Console.Write(name+", ");
            }
            System.Console.WriteLine("\n----------------------names before remove: ");
            foreach(var name in names)
            {
                System.Console.Write(name+", ");
            }
            names.Remove("Bob");
            System.Console.WriteLine("\n----------------------names after remove: ");
            foreach(var name in names)
            {
                System.Console.Write(name+", ");
            }

            Dictionary<int,int>mp = new Dictionary<int,int>(); 
            // int can be replaced with other data typesint, bool, double
            // string
            // char
            // enum
            // custom classes / structs
            // even collections like List<int>
            mp[1]=100;
            mp[3]=33;
            mp[4]=44;
            foreach(var it in mp)
            {
                System.Console.Write($"key: {it.Key} value: {it.Value}, ");// to access key values
            }
            System.Console.WriteLine($"\ncapacity of mp: {mp.Capacity}");//  return no of key values pairs   
            System.Console.WriteLine($"checked presence of key 3: {mp.ContainsKey(3)}");     

            Stack<int> st = new Stack<int>();
            st.Push(5);
            st.Push(4);
            st.Push(2);
            st.Pop();
            System.Console.WriteLine($"Top element of stack: {st.Peek()}");

            Queue<int>q = new Queue<int>();
            q.Enqueue(33);
            q.Enqueue(55);
            q.Enqueue(44);
            q.Dequeue();
            System.Console.WriteLine($"Front element of queue: {q.Peek()}");
            // peek return front value of queue
        }
    }
}