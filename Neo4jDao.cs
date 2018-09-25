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
                        var r = tx.Run("match (n)-[t]->(a) return n.name,a.name, t.weight");
                        return r ;
                    });

                    foreach (var n in transactions)
                    {
                        Transaction t = new Transaction();
                        var from = n["n.name"].As<string>();
                        var to = n["a.name"].As<string>();
                        var weight = n["t.weight"].As<int>();
                        t.from = from;
                        t.to = to;
                        t.weight = weight;
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
                    //get all the transactions, with source and destination nodes
                    var transactionBlock = session.WriteTransaction(tx =>
                    {
                        var r = tx.Run("match (n)-[t]->(a) return n.name,a.name, t.weight");
                        return r ;
                    });

                   

                    // a transaction is a neighbor both for its source and its destination,
                    // and an exit only for its destination
                    foreach (var trBlock in transactionBlock)
                    {
                        var nodeS = new Node();
                        var nodeD = new Node();
                        
                        var nameS = trBlock["n.name"].As<string>();
                        nodeS.name = nameS;
                        var nameD = trBlock["a.name"].As<string>();
                        nodeD.name = nameD;
                        var weight = trBlock["t.weight"].As<int>();
                        Transaction t = new Transaction();
                        t.from = nameS;
                        t.to = nameD;
                        t.weight = weight;
                        if( ! result.ContainsKey(nameS) )
                        {
                            nodeS.exits.Add( t );
                            nodeS.neighbors.Add( t );
                            result.Add( nameS , nodeS);
                            //Console.WriteLine(" nuovo, nodo: " + nameS + " neigh: " + t);
                        }
                        else
                        {
                            result[nameS].exits.Add(t);
                            result[nameS].neighbors.Add(t);
                            //Console.WriteLine(" vecchio, nodo: " + nameS + " neigh: " + t);
                        }
                        if( ! result.ContainsKey(nameD) )
                        {
                            nodeD.neighbors.Add( t );
                            result.Add( nameD , nodeD);
                            //Console.WriteLine(" nuovo dest, nodo: " + nameD + " neigh: " + t);
                        }
                        else
                        {
                            result[nameD].neighbors.Add(t);
                            //Console.WriteLine(" vecchio dest, nodo: " + nameD + " neigh: " + t);
                        }
                    }
                }

            }
            return result;
        }
    }
}
