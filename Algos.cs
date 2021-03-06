using System;
using Neo4j.Driver.V1;
using graph;
using System.Collections.Generic;
using System.Linq;

namespace dao
{
    //this class implements some way to explore the graph
    class Algos
    {

        private List<String> visited { get; set; } = new List<String>() ;
        private List<String> recStack { get; set; } = new List<String>() ;
               private Dictionary<String,Node> nodes { get; set; } = new Dictionary<String,Node>() ;
        private List<Transaction> transactions { get; set; } = new List<Transaction>() ;

        public Algos( Dictionary<String,Node> _nodes,
             List<Transaction> _transactions  )
        {
            this.nodes = _nodes;
            this.transactions = _transactions;
        }


                
        public void minimumSpanningTree( )
        {
            List<Transaction> ts = transactions;
            ts = ts.OrderBy( t => t.weight ).ToList();
            HashSet<String> linked = new HashSet<String>();
            foreach( var t in ts )
            {
                if( !linked.Contains(t.from) || !linked.Contains(t.to)   )
                {
                    linked.Add(t.from);
                    linked.Add(t.to);
                    Console.WriteLine( "transaction:" + t.ToString() );
                }
            }

        }

        public void dijkstra( string sroot )
        {
            Dictionary<String,int> reached = new Dictionary<String,int>();
            reached[sroot] = 0;
            Node root = nodes[sroot];
            List<Node> frontier = new List<Node>();
            frontier.Add( root );

            while( frontier.Count > 0 )
            {
                List<Node> actualFrontier = new List<Node>(frontier); //I wanna copy value
                foreach( Node node in actualFrontier )
                {
                    foreach( Transaction t in node.exits )
                    {
                        frontier.Add( nodes[t.to] );
                        if( reached.Keys.Contains(t.to) )
                        {
                            int toReach = t.weight + reached[node.name];
                            if( reached[t.to] > toReach )
                            {
                                reached[t.to] = toReach;
                            }
                        }
                        else{
                            int toReach = t.weight + reached[node.name];
                            reached[t.to] = toReach;
                        }
                    }
                    frontier.Remove( node );
                }

            }

            foreach( KeyValuePair<string, int> entry in reached )
            {
                Console.WriteLine("you can reach node: " + entry.Key + " in weight: " + entry.Value );
            }


            
        }

        public bool directGraphCheckForCycle(  )
        {
            List<String> visited = new List<String>();
            List<String> recStack = new List<String>();

            foreach(KeyValuePair<string, Node> entry in nodes)
            {
                if ( !visited.Contains( entry.Key))
                {
                    if ( isCyclicUtil(entry.Value, visited, recStack) )
                    {
                        Console.WriteLine("Is cyclic");
                        return true;
                    }
                } 
            
            }
            Console.WriteLine("Is NOT cyclic");    
            return false;
        }

        private List<Node> getChildNodes( Node n )
        {
            List<Node> results = new List<Node>();
            foreach( Transaction node in n.exits )
            {
                results.Add( nodes[node.to] );
            }
            return results;
        }

        private bool isCyclicUtil( Node v, List<String>visited, List<String>recStack)
        {
            visited.Add( v.name );
            recStack.Add(v.name);
            
            List<Node> neigs = new List<Node>();
            neigs = getChildNodes( v );
            foreach ( Node neighbour in neigs )
            {
                if ( !visited.Contains(neighbour.name) )
                {
                    if (isCyclicUtil(neighbour, visited, recStack) == true )
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

    }
}