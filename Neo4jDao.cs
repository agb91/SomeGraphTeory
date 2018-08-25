using System;
using Neo4j.Driver.V1;
using graph;
using System.Collections.Generic;
using System.Linq;

namespace dao
{
    class Neo4jDao
    {

        private List<String> visited = new List<String>();
        
     


        
 
        

        public List<Transaction> getTransactions()
        {
            List<Transaction> results = new List<Transaction>();

            IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687"
                , AuthTokens.Basic("neo4j", "root"));
    
            using( _driver )
            {
                using (var session = _driver.Session())
                {
                    //get all the nodes
                    var transactions = session.WriteTransaction(tx =>
                    {
                        var r = tx.Run("match (n)-[t]->(a) return n.name,a.name");
                        return r ;
                    });

                    foreach (var n in transactions)
                    {
                        Transaction t = new Transaction();
                        var from = n["n.name"].As<string>();
                        var to = n["a.name"].As<string>();
                        t.from = from;
                        t.to = to;
                        results.Add(t);
                    }
                }
            }

            return results;

            
        }

        public Dictionary<String,Node> getNodes()
        {

            Dictionary<String,Node> result = new  Dictionary<String,Node>();

            IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687"
                , AuthTokens.Basic("neo4j", "root"));
    
            using( _driver )
            {
                using (var session = _driver.Session())
                {
                    //get all the nodes
                    var nodes = session.WriteTransaction(tx =>
                    {
                        var r = tx.Run("match (n)-[t]->(a) return n.name,a.name");
                        return r ;
                    });

                   

                    //for each of them set the transactions
                    foreach (var n in nodes)
                    {
                        var node = new Node();
                        
                        var name = n["n.name"].As<string>();
                        node.name = name;
                        var to = n["a.name"].As<string>();
                        Transaction t = new Transaction();
                        t.from = name;
                        t.to = to;
                        node.exits.Add( t );
                        if( ! result.ContainsKey(name) )
                        {
                            result.Add( name , node);
                        }
                        else
                        {
                            result[name].exits.Add(t);
                        }

                        //add leaves
                        if( ! result.ContainsKey(to) )
                        {
                            result.Add( to , new Node(to));
                        }
                        
                    }
                }

            }
            return result;
        }
    }
}
