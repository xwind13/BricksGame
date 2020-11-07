using System;
using System.Collections.Generic;

namespace BricksGame.Logic.Matrix
{
    public abstract class ChainLink<T, TD> : IChainLink<T, TD> 
        where TD : IConvertible
    {
        protected readonly Dictionary<TD, IChainLink<T, TD>> _links;

        public T Value { get; private set; }

        public ChainLink(T value)
        {
            _links = new Dictionary<TD, IChainLink<T, TD>>();

            Value = value;
        }

        public IChainLink<T, TD> Next(TD direction) => _links[direction];
    }
}
