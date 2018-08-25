using System;
using Neo4j.Driver.V1;
using graph;
using System.Collections.Generic;
using System.Linq;

namespace dao
{
    
    //it is a factory class
    class GraphManager
    {
        private Dictionary<String,Node> nodes { get; set; } = new Dictionary<String,Node>() ;
        private List<Transaction> transactions { get; set; } = new List<Transaction>() ;



        public GraphManager( Dictionary<String,Node> _nodes,
             List<Transaction> _transactions )
        {
            this.nodes = _nodes;
            this.transactions = _transactions;
        }

        public Searches getSearcher()
        {
            return new Searches( nodes, transactions );
        }

        public Algos getAlgos()
        {
            return new Algos( nodes, transactions );
        }

    }
}
