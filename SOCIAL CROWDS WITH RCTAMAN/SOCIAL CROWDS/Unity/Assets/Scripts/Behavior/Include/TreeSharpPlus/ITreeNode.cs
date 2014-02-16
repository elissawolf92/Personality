using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharpPlus
{
    public interface ITreeNode
    {
        RunStatus? LastStatus { get; }
        ITreeNode Parent { get; }
        Guid Guid { get; }

        bool Equals(ITreeNode other);
        bool Equals(object other);

        int GetHashCode();

        IEnumerable<RunStatus> Execute();

        RunStatus Tick();
        void Start();
        void Stop();
    }
}
