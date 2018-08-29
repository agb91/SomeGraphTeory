using System;
using Neo4j.Driver.V1;
using graph;
using System.Collections.Generic;
using dao;

namespace main
{
    class Program
    {
        static void Main(string[] args)
        {
            Neo4jDao dao = new Neo4jDao();
            
            Dictionary<String,Node> nodes = dao.getNodes();
            List<Transaction> transactions = dao.getTransactions();

            /*foreach(KeyValuePair<string, Node> entry in nodes)
            {
                Console.WriteLine( "node found: " 
                    + entry.Value.ToString() );
            };*/

            /*foreach( var t in transactions )
            {
                Console.WriteLine("from: " + t.from + " --> to: " + t.to + "; weight:" + t.weight);
            }*/

            GraphManager gm = new GraphManager( nodes, transactions );
            Searches search = gm.getSearcher();
            Algos algos = gm.getAlgos();


            

            //try breadth first
            //search.breadthFirst( nodes["aaa"],  nodes );//recursive
            //search.breadthFirstUsingQueue( nodes["aaa"] );//not recursive
            //search.deepFirst( nodes["aaa"],  nodes );//recursive
            //search.deepFirstUsingStack(  nodes["aaa"] );//not recursive
            //algos.minimumSpanningTree( );
            //algos.directGraphCheckForCycle( );
            algos.dijkstra("aaa");
            Console.WriteLine("Finished");
            //Console.ReadLine();
        }
    }
}
