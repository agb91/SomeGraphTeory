using System;
using Neo4j.Driver.V1;
using graph;
using System.Collections.Generic;
using System.Linq;

namespace dao
{
    class GraphManager
    {

        private List<String> visited { get; set; } = new List<String>() ;
        private List<String> recStack { get; set; } = new List<String>() ;
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

    }
}
