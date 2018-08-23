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
        
        //recoursive first call
        public void breadthFirst( Node root, Dictionary<String,Node> graph )
        {
            visited.Clear();
            List<Node> roots = new List<Node>();
            roots.Add(root);
            visited.Add(root.name);

            breadthFirst( roots, graph );
        }

        //recoursive
        public void breadthFirst( List<Node> thisLevel, Dictionary<String,Node> graph )
        {
            graph = new Dictionary<string, Node>(graph);//wanna pass by valueeee
            if( thisLevel.Count == 0 )
            {
                return;
            }

            //print this gen
            foreach( var n in thisLevel )
            {
                Console.WriteLine("Visited:" + n.name);
                graph.Remove( n.name );
            }

            //get next gen
            List<Node> newGen = new List<Node>();
            foreach ( var n in thisLevel )
            {
                foreach( var son in n.exits )
                {
                    if( graph.ContainsKey(son.to) && !visited.Contains(son.to)  )
                    {
                        newGen.Add( graph[son.to] );
                    }
                }
            }
            //recoursive
            breadthFirst( newGen, graph );    
     
        }

        //recoursive first call
        public void deepFirst( Node root, Dictionary<String,Node> graph )
        {
            visited.Clear();
            Console.WriteLine("visited: " + root.name);
            visited.Add(root.name);
            List<Node> path = new List<Node>();
            path.Add(root);
            graph = new Dictionary<string, Node>(graph);//wanna pass by valueeee

            deepFirst( path, graph );
        }
    
        //recoursive
        public void deepFirst( List<Node> path, Dictionary<String,Node> graph )
        {
            int last = path.Count - 1;
            Node thisNode = path[ last ];

            foreach( Transaction tSon in thisNode.exits )
            {
                Node son = graph[tSon.to];
                if( !path.Contains(son) && !visited.Contains( son.name ))
                {
                    Console.WriteLine("visited: " + son.name);
                    visited.Add(son.name);
                    List<Node> thisPath = new List<Node>();
                    thisPath.AddRange( path );
                    thisPath.Add( son );
                    deepFirst( thisPath,  graph );
                }
            }
    
        }

        //not recoursive
        public void breadthFirstUsingQueue( Node root, Dictionary<String,Node> graph )
        {
            visited.Clear();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(root);

            while( queue.Count>0 )
            {
                Node n = queue.Dequeue();
                string name = n.name;
                if( !visited.Contains( name ) )
                {
                    Console.WriteLine( name );
                    visited.Add( name );
                    foreach( var sonName in n.exits )
                    {
                        Node son = graph[sonName.to];
                        queue.Enqueue(son);
                    }
                }
                
            }

        }

        //not recoursive
        public void deepFirstUsingStack( Node root, Dictionary<String,Node> graph )
        {
            visited.Clear();

            Stack<Node> stack = new Stack<Node>();
            stack.Push(root);

            while( stack.Count>0 )
            {
                Node n = stack.Pop();
                string name = n.name;
                if( !visited.Contains( name ) )
                {
                    Console.WriteLine( name );
                    visited.Add( name );
                    foreach( var sonName in n.exits )
                    {
                        Node son = graph[sonName.to];
                        stack.Push(son);
                    }
                }
                
            }

        }
        
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
