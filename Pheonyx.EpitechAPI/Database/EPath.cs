using System;
using System.Collections.Generic;
using System.Linq;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI.Database
{

    public sealed class EPath
    {
        private static readonly Int64 Start = -0x2;
        private static readonly Int64 End = -0x4;

        private long _currentPath;
        private readonly long _pathSize;
        private readonly string _originPath;
        private readonly string[] _pathArray;

        public EPath(String sPath)
        {
            sPath.ArgumentNotEmpty(nameof(sPath));
            _originPath = sPath;
            var lPathList = sPath.Split('.').ToList();

            for (var i = 0; i < lPathList.Count; i++)
            {
                var subPath = lPathList[i];
                if (subPath == String.Empty)
                    throw new ArgumentException($"Invalid path: Key {i + 1} can't be empty in '{sPath}'.", nameof(sPath));
                while (subPath.Contains('[', ']'))
                {
                    var arrayPath = subPath.LastBetween('[', ']');
                    int iOut;

                    if (!Int32.TryParse(arrayPath, out iOut))
                        throw new ArgumentException($"Invalid path: Incorrect Array key '[{arrayPath}]' in '{sPath}'. Key must be of type Int32.", nameof(sPath));
                    lPathList.Insert(i + 1, subPath.LastBetween('[', ']'));
                    lPathList[i] = subPath.Substring(0, subPath.LastIndexOf('[')) + subPath.Substring(subPath.LastIndexOf(']') + 1);

                    subPath = lPathList[i];
                    if (subPath == String.Empty)
                        throw new ArgumentException($"Invalid path: Key {i + 1} can't be empty in '{sPath}'.", nameof(sPath));
                }
            }
            _pathArray = lPathList.ToArray();
            _pathSize = lPathList.Count;
            _currentPath = Start;
        }

        public String OriginPath => _originPath;
        public String CurrentPath => _currentPath < 0 ? null : _pathArray[_currentPath];

        public Int32? CurrentRow
        {
            get
            {
                int row;

                if (Int32.TryParse(CurrentPath, out row))
                    return row;
                return null;
            }
        }
        public Boolean MoveNext()
        {
            if (_currentPath >= _pathSize - 1 || _currentPath == End)
                _currentPath = End;
            else if (_currentPath == Start)
                _currentPath = 0;
            else
                _currentPath++;
            return _currentPath != End;
        }
        public Boolean MovePrev()
        {
            if (_currentPath == 0 || _currentPath == Start)
                _currentPath = Start;
            else if (_currentPath == End)
                _currentPath = _pathSize - 1;
            else
                _currentPath--;
            return _currentPath != Start;
        }

        public void Reset()
        {
            _currentPath = Start;
        }

        public override String ToString()
        {
            return _originPath;
        }

        public static implicit operator EPath(String sPath)
        {
            return new EPath(sPath);
        }
        public static implicit operator String(EPath ePath)
        {
            return ePath.ToString();
        }
    }
}