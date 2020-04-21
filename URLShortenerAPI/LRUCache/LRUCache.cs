using System;
using System.Collections.Generic;

namespace URLShortenerAPI.LRUCache
{
    public class LRUCache : ICache
    {
        private int capacity;
        private int count;
        private Dictionary<string, LRUNode> nodeMap;
        private LRUDoubleLinkedList doubledLinkedList;
        public LRUCache(int capacity)
        {
            this.capacity = capacity;
            this.count = 0;
            nodeMap = new Dictionary<string, LRUNode>();
            doubledLinkedList = new LRUDoubleLinkedList();
        }

        public int Count()
        {
            return count;
        }

        public string Get(string hashID)
        {
            if (!nodeMap.ContainsKey(hashID))
            {
                return null;
            }
            if (string.IsNullOrEmpty(hashID) || string.IsNullOrWhiteSpace(hashID))
            {
                throw new ArgumentException("HashID cannot be null, empty or white spaces");
            }
            LRUNode existingNode = nodeMap[hashID];
            // Move node to front of list when read.
            doubledLinkedList.RemoveNode(existingNode);
            doubledLinkedList.AddToFront(existingNode);
            return existingNode.RedirectUrl;
        }

        public void Add(string hashID, string redirectUrl)
        {
            if (string.IsNullOrEmpty(hashID) || string.IsNullOrWhiteSpace(hashID))
            {
                throw new ArgumentException("HashID cannot be null, empty or white spaces");
            }
            if (string.IsNullOrEmpty(redirectUrl) || string.IsNullOrWhiteSpace(redirectUrl))
            {
                throw new ArgumentException("Redirect URL cannot be null, empty or white spaces");
            }
            if (nodeMap.ContainsKey(hashID))
            {
                // Update existing node's value.
                LRUNode existingNode = nodeMap[hashID];
                doubledLinkedList.RemoveNode(existingNode);
                existingNode.RedirectUrl = redirectUrl;
                doubledLinkedList.AddToFront(existingNode);
            }
            else
            {
                // LRU cache is full, removing least recently used node
                // from map and list.
                if (count == capacity)
                {
                    LRUNode removedNode = doubledLinkedList.RemoveLRUNode();
                    nodeMap.Remove(removedNode.HashID);
                    count--;
                }
                LRUNode newNode = new LRUNode(hashID, redirectUrl);
                doubledLinkedList.AddToFront(newNode);
                nodeMap[hashID] = newNode;
                count++;
            }
        }
    }
}
