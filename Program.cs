﻿using System;
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
                Console.WriteLine( "node found: " + " -> " 
                    + entry.Value.ToString() );
            };*/

            /*foreach( var t in transactions )
            {
                Console.WriteLine("from: " + t.from + " --> to: " + t.to);
            }*/

            GraphManager gm = new GraphManager( nodes, transactions );
            Searches search = gm.getSearcher();


            

            //try breadth first
            //dao.breadthFirst( nodes["aaa"],  nodes );//recursive
            //dao.breadthFirstUsingQueue( nodes["aaa"], nodes );//not recursive
            //dao.deepFirst( nodes["aaa"],  nodes );//recursive
            //dao.deepFirstUsingStack(  nodes["aaa"], nodes  );//not recursive
            //dao.minimumSpanningTree( transactions );
            dao.directGraphCheckForCycle( nodes, transactions );
            Console.WriteLine("Finished");
            //Console.ReadLine();
        }
    }
}
