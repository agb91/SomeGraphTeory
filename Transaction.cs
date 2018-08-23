namespace graph
{
    class Transaction
    {
        public string from { get; set; } = "";
        public string to { get; set; } = "";

        public int weight = 1;

        public override string ToString()
        {
            return from + "->" + to ;
        }

    }

}    