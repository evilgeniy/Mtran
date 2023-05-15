using System.Collections.Generic;
 

namespace ConsoleApp1
{
    public class Tree<T> : List<T>
    {
        public T L = default;
        public T R = default;
        public Tree<T> Parents = null;
        public Tree<T> Block = null;
        public int Idention;

        public Tree(Tree<T> TreeL)
        {
            Block = TreeL;
        }


    }
}