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
        
     
        
        public void minimumSpanningTree( List<Transaction> ts )
        {
            ts = ts.OrderBy( t => t.weight ).ToList();
            HashSet<String> linked = new HashSet<String>();
            foreach( var t in ts )
            {
                if( !linked.Contains(t.from) || !linked.Contains(t.to)   )
                {
                    linked.Add(t.from);
                    linked.Add(t.to);
                    Console.WriteLine( "transacion:" + t.ToString() );
                }
            }

        }

        public bool directGraphCheckForCycle( Dictionary<String,Node> nodes
            , List<Transaction> transaction )
        {
            List<String> visited = new List<String>();
            List<String> recStack = new List<String>();

            foreach(KeyValuePair<string, Node> entry in nodes)
                if ( !visited.Contains( entry.Key))
                {
                    if ( isCyclicUtil(entry.Value, visited, recStack, nodes) )
                    {
                        return true;
                    }
                } 
            return false;
        }

        private List<Node> getChildNodes( Node n, Dictionary<String, Node> nodes )
        {
            List<Node> results = new List<Node>();
            foreach( Transaction node in n.exits )
            {
                results.Add( nodes[node.to] );
            }
            return results;
        }

        private bool isCyclicUtil( Node v, List<String>visited, List<String>recStack, 
        Dictionary<String, Node> nodes)
        {
            visited.Add( v.name );
            recStack.Add(v.name);
            
            List<Node> neigs = new List<Node>();
            neigs = getChildNodes( v, nodes );
            foreach ( Node neighbour in neigs )
            {
                if ( !visited.Contains(neighbour.name) )
                {
                    if (isCyclicUtil(neighbour, visited, recStack, nodes) == true )
                        return true;
                }
                else if( recStack.Contains(neighbour.name) )
                {
                    return true;
                }      
            }

            recStack.Remove(v.name);
            return false;
        }
 
        

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
