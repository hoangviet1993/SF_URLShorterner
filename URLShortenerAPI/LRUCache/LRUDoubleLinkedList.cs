using System;

namespace URLShortenerAPI.LRUCache
{
    public class LRUDoubleLinkedList
    {
        private LRUNode Head;
        private LRUNode Tail;

        public LRUDoubleLinkedList()
        {
            Head = new LRUNode();
            Tail = new LRUNode();
            Head.Next = Tail;
            Tail.Previous = Head;
        }

        public void AddToFront(LRUNode node)
        {
            if (node == null)
            {
                throw new ArgumentException("New node cannot be null.");
            }
            node.Next = Head.Next;
            Head.Next.Previous = node;
            node.Previous = Head;
            Head.Next = node;
        }

        public void RemoveNode(LRUNode node)
        {
            if (node == null)
            {
                throw new ArgumentException("Cannot remove null node.");
            }
            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
            node.Next = null;
            node.Previous = null;
        }

        public LRUNode RemoveLRUNode()
        {
            LRUNode target = Tail.Previous;
            if (target == null)
            {
                throw new InvalidOperationException("Cannot remove node when list is empty.");
            }
            RemoveNode(target);
            return target;
        }
    }
}
