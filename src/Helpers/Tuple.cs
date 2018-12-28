namespace CSM.Helpers
{
    public class Tuple<T1, T2, T3>
    {
        public T1 Var1 { get; set; }
        public T2 Var2 { get; set; }
        public T3 Var3 { get; set; }

        public Tuple(T1 obj1, T2 obj2, T3 obj3)
        {
            Var1 = obj1;
            Var2 = obj2;
            Var3 = obj3;
        }
    }
}
