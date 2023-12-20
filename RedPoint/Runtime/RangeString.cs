using System;

namespace UniWork.RedPoint.Runtime
{
    /**
     * 表示在源字符串中，从 StartIndex 到 EndIndex 范围的字符构成的字符串
     */
    public struct RangeString : IEquatable<RangeString>
    {
        private string _source;
        private int _startIndex;
        private int _endIndex;
        private int _length;
        private int _hashCode;

        public RangeString(string source, int startIndex, int endIndex)
        {
            _source = source;
            _startIndex = startIndex;
            _endIndex = endIndex;
            _length = endIndex - startIndex + 1;
            _hashCode = 0;

            if (string.IsNullOrEmpty(source) == false)
            {
                for (int i = startIndex; i <= endIndex; ++i)
                    _hashCode = 31 * _hashCode + source[i];
            }
        }
        
        public bool Equals(RangeString other)
        {
            bool selfNullOrEmpty = string.IsNullOrEmpty(_source);
            bool otherNullOrEmpty = string.IsNullOrEmpty(other._source);

            if (selfNullOrEmpty && otherNullOrEmpty)
                return true;

            if (selfNullOrEmpty || otherNullOrEmpty)
                return false;

            if (_length != other._length)
                return false;

            for (int i = _startIndex, j = other._startIndex; i <= _endIndex; ++i, ++j)
            {
                if (_source[i] != other._source[j])
                    return false;
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            RedPointTree.CachedSb.Clear();
            for (int i = _startIndex; i <= _endIndex; ++i)
                RedPointTree.CachedSb.Append(_source[i]);

            return RedPointTree.CachedSb.ToString();
        }
    }
}