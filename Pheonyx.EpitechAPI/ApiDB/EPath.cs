using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pheonyx.EpitechAPI
{

    public sealed class EPath
    {
        private static readonly long Start = -0x2;
        private static readonly long End = -0x4;

        private string originPath;
        private long currentPath;
        private string[] pathArray;
        private long pathSize;

        public EPath(String sPath)
        {
            sPath.ArgumentNotEmpty(nameof(sPath));
            originPath = sPath;
            List<String> lPathList = sPath.Split('.').ToList();

            for (int i = 0; i < lPathList.Count; i++)
            {
                string subPath = lPathList[i];
                if (subPath == String.Empty)
                    throw new ArgumentException(String.Format(String.Format("Invalid path: Key {0} can't be empty in '{1}'.", i + 1, sPath), nameof(sPath)));
                while (subPath.Contains('[', ']'))
                {
                    string arrayPath = subPath.LastBetween('[', ']');
                    int iOut;

                    if (!Int32.TryParse(arrayPath, out iOut))
                        throw new ArgumentException(String.Format("Invalid path: Incorrect Array key '[{0}]' in '{1}'. Key must be of type Int32.", arrayPath, sPath), nameof(sPath));
                    lPathList.Insert(i + 1, subPath.LastBetween('[', ']'));
                    lPathList[i] = subPath.Substring(0, subPath.LastIndexOf('[')) + subPath.Substring(subPath.LastIndexOf(']') + 1);

                    subPath = lPathList[i];
                    if (subPath == String.Empty)
                        throw new ArgumentException(String.Format(String.Format("Invalid path: Key {0} can't be empty in '{1}'.", i + 1, sPath), nameof(sPath)));
                }
            }
            pathArray = lPathList.ToArray();
            pathSize = lPathList.Count;
            currentPath = Start;
        }

        public String OriginPath
        {
            get
            {
                return originPath;
            }
        }
        public String CurrentPath
        {
            get
            {
                if (currentPath < 0)
                    return (null);
                return pathArray[currentPath];
            }
        }
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

        public bool MoveNext()
        {
            if (currentPath >= pathSize - 1 || currentPath == End)
                currentPath = End;
            else if (currentPath == Start)
                currentPath = 0;
            else
                currentPath++;
            return (currentPath != End);
        }
        public bool MovePrev()
        {
            if (currentPath == 0 || currentPath == Start)
                currentPath = Start;
            else if (currentPath == End)
                currentPath = pathSize - 1;
            else
                currentPath--;
            return (currentPath != Start);
        }

        public void Reset()
        {
            currentPath = Start;
        }

        public override String ToString()
        {
            return originPath;
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