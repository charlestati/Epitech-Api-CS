using System;
using System.Collections.Generic;
using System.Linq;

namespace Pheonyx.EpitechAPI.Utils
{
    public static class StringExtension
    {
        /// <summary>
        ///     Génère une exception de type <see cref="ArgumentException" /> si la chaîne de caractère <paramref name="self" />
        ///     est vide.
        /// </summary>
        /// <param name="self">Instance de la chaîne courante.</param>
        /// <param name="nameArgument">Nom de l'argument.</param>
        public static void ArgumentNotEmpty(this string self, string nameArgument)
        {
            self.ArgumentNotNull(nameArgument);
            if (self == string.Empty)
                throw new ArgumentException($"String {nameArgument} can't be empty.", nameArgument);
        }

        /// <summary>
        ///     Retourne une valeur qui indique si le(s) caractère(s) <see cref="Char" /> spécifié(s) apparaissent tous dans cette
        ///     chaine.
        /// </summary>
        /// <param name="self">Instance courante de la chaine de caractères.</param>
        /// <param name="needle">Liste des caractères à vérifier.</param>
        /// <returns>
        ///     <c>true</c> si <paramref name="self" /> contient tous les caractères spécifiés dans <paramref name="needle" />
        ///     .
        /// </returns>
        public static bool Contains(this string self, params char[] needle)
        {
            return needle.All(c => self.IndexOf(c) != -1);
        }

        /// <summary>
        ///     Retourne une chaine <see cref="String" /> contenu dans cette chaîne entre le caractères <paramref name="start" />
        ///     et <paramref name="end" />.
        /// </summary>
        /// <param name="self">Instance de la chaîne courante.</param>
        /// <param name="start">Caractère de début.</param>
        /// <param name="end">Caractère de fin.</param>
        /// <returns>
        ///     Chaine équivalente à la sous-chaîne qui commence à <paramref name="start" /> et se termine à
        ///     <paramref name="end" />.
        /// </returns>
        public static string Between(this string self, char start, char end)
        {
            if (!self.Contains(start, end))
                return self;
            return self.Substring(self.IndexOf(start) + 1, self.IndexOf(end) - (self.IndexOf(start) + 1));
        }

        /// <summary>
        ///     Retourne la dernière occurance d'une chaine <see cref="String" /> contenu dans cette chaine entre le caractère
        ///     <paramref name="start" /> et <paramref name="end" />.
        /// </summary>
        /// <param name="self">Instance de la chaîne courante.</param>
        /// <param name="start">Caractère de début.</param>
        /// <param name="end">Caractère de fin.</param>
        /// <returns>
        ///     Chaine équivalente à la dernière occurence de la sous-chaîne qui commence à <paramref name="start" /> et se
        ///     termine à <paramref name="end" />.
        /// </returns>
        public static string LastBetween(this string self, char start, char end)
        {
            if (!self.Contains(start, end) && self.LastIndexOf(start) < self.LastIndexOf(end))
                return self;
            return self.Substring(self.LastIndexOf(start) + 1, self.LastIndexOf(end) - (self.LastIndexOf(start) + 1));
        }
    }

    public static class ObjectExtension
    {
        /// <summary>
        ///     Génère une exception de type <see cref="ArgumentNullException" /> si l'instance de l'objet est nulle.
        /// </summary>
        /// <param name="self">Instance de l'objet courant.</param>
        /// <param name="nameArgument">Nom de l'argument.</param>
        public static void ArgumentNotNull(this object self, string nameArgument)
        {
            if (self == null)
                throw new ArgumentNullException(nameArgument);
        }

        /// <summary>
        ///     Génère une exception de type <see cref="ArgumentException" /> si le type de l'instance de l'objet est invalide.
        /// </summary>
        /// <param name="self">Instance de l'objet courant.</param>
        /// <param name="type"><see cref="Type" /> de vérification.</param>
        /// <param name="nameArgument">Nom de l'argument.</param>
        public static void ArgumentValidType(this object self, Type type, string nameArgument)
        {
            if (self.GetType() != type)
                throw new ArgumentException(
                    $"Argument '{nameArgument}' has invalid type ({self.GetType()}). {type} was expected.");
        }
    }

    public static class EnumerableExtension
    {
        /// <summary>
        ///     Retourne une valeur qui indique si le <see cref="IEnumerable{T}" /> est vide.
        /// </summary>
        /// <typeparam name="T"><see cref="Type" /> de valeur contenu dans <paramref name="self" /></typeparam>
        /// <param name="self">Instance courante de <see cref="IEnumerable{T}" />.</param>
        /// <returns><c>true</c> si <paramref name="self" /> est vide.</returns>
        public static bool Empty<T>(this IEnumerable<T> self)
        {
            return !self.Any();
        }

        /// <summary>
        ///     Retourne une valeur qui indique si le <see cref="IEnumerable{T}" /> n'est pas vide.
        /// </summary>
        /// <typeparam name="T"><see cref="Type" /> de valeur contenu dans <paramref name="self" /></typeparam>
        /// <param name="self">Instance courante de <see cref="IEnumerable{T}" />.</param>
        /// <returns><c>true</c> si <paramref name="self" /> n'est pas vide.</returns>
        public static bool NotEmpty<T>(this IEnumerable<T> self)
        {
            return !self.Empty();
        }
    }
}