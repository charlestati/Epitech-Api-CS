using System;
using System.Linq;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI.Database
{
    public sealed class EPath
    {
        /// <summary>
        /// Variable statique représentant l'état noeud initial.
        /// </summary>
        public static readonly Int32 Start = -0x2;
        /// <summary>
        /// Variable statique représentant l'état noeud final.
        /// </summary>
        public static readonly Int32 End = -0x4;
        private readonly String[] _pathArray;
        private readonly long _pathSize;

        private long _currentPath;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EPath"/> qui est représenté par le chemin specifié.
        /// </summary>
        /// <param name="sPath">Chemin que doit parcourir l'instance <see cref="EPath"/>.</param>
        public EPath(String sPath)
        {
            sPath.ArgumentNotEmpty(nameof(sPath));
            OriginPath = sPath;
            var lPathList = sPath.Split('.').ToList();

            for (var i = 0; i < lPathList.Count; i++)
            {
                var subPath = lPathList[i];
                if (subPath == string.Empty)
                    throw new ArgumentException($"Invalid path: Key {i + 1} can't be empty in '{sPath}'.", nameof(sPath));
                while (subPath.Contains('[', ']'))
                {
                    var arrayPath = subPath.LastBetween('[', ']');
                    int iOut;

                    if (!int.TryParse(arrayPath, out iOut))
                        throw new ArgumentException(
                            $"Invalid path: Incorrect Array key '[{arrayPath}]' in '{sPath}'. Key must be of type Int32.",
                            nameof(sPath));
                    lPathList.Insert(i + 1, subPath.LastBetween('[', ']'));
                    lPathList[i] = subPath.Substring(0, subPath.LastIndexOf('[')) +
                                   subPath.Substring(subPath.LastIndexOf(']') + 1);

                    subPath = lPathList[i];
                    if (subPath == string.Empty)
                        throw new ArgumentException($"Invalid path: Key {i + 1} can't be empty in '{sPath}'.",
                            nameof(sPath));
                }
            }
            _pathArray = lPathList.ToArray();
            _pathSize = lPathList.Count;
            _currentPath = Start;
        }

        public override String ToString()
        {
            return OriginPath;
        }

        public static implicit operator EPath(String sPath)
        {
            return new EPath(sPath);
        }

        public static implicit operator String(EPath ePath)
        {
            return ePath.ToString();
        }

        #region Properties

        /// <summary>
        /// Obtient une chaîne représentant le chemin original de l'instance.
        /// </summary>
        public String OriginPath { get; }

        /// <summary>
        /// Obtient une chaîne représentant l'état du chemin actuel.
        /// </summary>
        public String CurrentPath => _currentPath < 0 ? null : _pathArray[_currentPath];

        #endregion Properties

        #region Move methods

        /// <summary>
        /// Obtient le numero du noeud actuel.
        /// </summary>
        public Int32 CurrentRow
        {
            get
            {
                int row;

                if (int.TryParse(CurrentPath, out row))
                    return row;
                return End;
            }
        }

        /// <summary>
        /// Déplace le noeud d'une unité vers l'avant.
        /// </summary>
        /// <returns><c>true</c> si le noeud est arrivé à la fin du chemin; sinon, <c>false</c></returns>
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

        /// <summary>
        /// Déplace le noeud d'une unité vers l'arrière.
        /// </summary>
        /// <returns><c>true</c> si le noeud est arrivé au début du chemin; sinon, <c>false</c></returns>
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

        /// <summary>
        /// Réinitialise le noeud actuel au début du chemin.
        /// </summary>
        public void Reset()
        {
            _currentPath = Start;
        }

        #endregion Move methods
    }
}