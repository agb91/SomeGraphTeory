using System;
using Neo4j.Driver.V1;
using graph;
using System.Collections.Generic;
using System.Linq;

namespace dao
{
    //this class implements some way to explore the graph
    class Searches
    {

        private List<String> visited { get; set; } = new List<String>() ;
        private List<String> recStack { get; set; } = new List<String>() ;
        private Dictionary<String,Node> nodes { get; set; } = new Dictionary<String,Node>() ;
        private List<Transaction> transactions { get; set; } = new List<Transaction>() ;

        public Searches( Dictionary<String,Node> _nodes,
             List<Transaction> _transactions  )
        {
            this.nodes = _nodes;
            this.transactions = _transactions;
        }

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
        public void breadthFirstUsingQueue( Node root)
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
                        Node son = nodes[sonName.to];
                        queue.Enqueue(son);
                    }
                }
                
            }

        }

        //not recoursive
        public void deepFirstUsingStack( Node root )
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
                        Node son = nodes[sonName.to];
                        stack.Push(son);
                    }
                }
                
            }

        }

        //binary search
        public void binarySearchNotOriented( Node r1 , Node r2 )
        {
            //todo
        }

    }
}