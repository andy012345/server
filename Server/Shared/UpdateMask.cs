using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdateMask
    {
        private int m_maxBlockCount;
        private uint[] m_blocks;

        protected internal int m_lowestIndex;
        protected internal int m_highestIndex;

        public UpdateMask(int highestField)
        {
            //m_maxBlockCount = (highestField + 31) >> 5;
            m_maxBlockCount = (highestField >> 5) + 1;
            Clear();
        }

        public int MaxBlockCount
        {
            get { return m_maxBlockCount; }
        }

        public uint[] Blocks
        {
            get { return m_blocks; }
        }

        public int LowestIndex
        {
            get { return m_lowestIndex; }
        }

        public int HighestIndex
        {
            get
            {
                return m_highestIndex;
            }
            set
            {
                m_maxBlockCount = (value >> 5) + 1;

                if (m_maxBlockCount > m_blocks.Length)
                {
                    Array.Resize(ref m_blocks, m_maxBlockCount);
                }

            }
        }

        public bool HasBitsSet
        {
            get
            {
                return m_highestIndex >= m_lowestIndex;
            }
        }

        public void Clear()
        {
            m_highestIndex = 0;
            m_lowestIndex = int.MaxValue;
            m_blocks = new uint[m_maxBlockCount];
        }

        public void UnsetBit(int index)
        {
            m_blocks[index >> 5] &= ~(uint)(1 << (index & 31));
        }

        public void SetAll()
        {
            for (int i = 0; i < m_maxBlockCount; i++)
            {
                m_blocks[i] = uint.MaxValue;
            }
        }

        public void SetBit(int index)
        {
            m_blocks[index >> 5] |= (uint)(1 << (index & 31));
            if (index > m_highestIndex)
            {
                m_highestIndex = index;
            }
            if (index < m_lowestIndex)
            {
                m_lowestIndex = index;
            }
        }

        public bool GetBit(int index)
        {
            return (m_blocks[index >> 5] & (uint)(1 << (index & 31))) != 0;
        }
    }
}
