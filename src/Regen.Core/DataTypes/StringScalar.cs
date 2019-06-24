using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Regen.DataTypes {
    [DebuggerDisplay("String: {" + nameof(Value) + "}")]
    public class StringScalar : Scalar, IComparable<string>, IEnumerable<char>, IEnumerable, IEquatable<StringScalar> {
        private string _value => base.Value as string;

        public override string Emit() {
            return (string) base.Value;
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            return $"\"{Emit()}\"";
        }

        /// <summary>
        ///     Converts all character to string.
        /// </summary>
        /// <returns></returns>
        public Array ToArray() {
            return Array.Create(((string) base.Value).ToCharArray());
        }

        public char this[int index] {
            get {
                try {
                    return ((string) base.Value)[index];
                } catch (IndexOutOfRangeException) {
                    return '\0';
                }
            }
        }


        public string this[int index, int count] {
            get => Substring(index, count);
            set {
                var str = base.Value as string;
                if (index < 0)
                    return;
                base.Value = str.Remove(index, Math.Min(count, str.Length - index)).Insert(index, value);
            }
        }

        public StringScalar(object value) : base(value) { }

        //Yes.. I know, I use the compiler to compile code for the compiler.
#if _REGEN
        %numericalTypes = ["String"|"Boolean"|"Byte"|"SByte"|"Int16"|"UInt16"|"Int32"|"UInt32"|"Int64"|"UInt64"|"IntPtr"|"UIntPtr"|"Char"|"Double"|"Single"|"Decimal"]
        %foreach numericalTypes%

        public static string operator +(StringScalar sc, #1 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(#1 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }
        %
#else

        public static string operator +(StringScalar sc, String rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(String v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Boolean rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Boolean v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Byte rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Byte v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, SByte rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(SByte v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Int16 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Int16 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, UInt16 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(UInt16 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Int32 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Int32 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, UInt32 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(UInt32 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Int64 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Int64 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, UInt64 rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(UInt64 v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, IntPtr rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(IntPtr v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, UIntPtr rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(UIntPtr v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Char rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Char v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Double rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Double v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Single rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Single v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }

        public static string operator +(StringScalar sc, Decimal rhs) {
            object lhs = (string) sc.Value;
            return lhs + rhs.ToString();
        }

        public static string operator +(Decimal v, StringScalar sc) {
            string rhs = (string) sc.Value;
            return v.ToString() + rhs;
        }
#endif

        #region Overloads

        /// <summary>Compares this instance with a specified <see cref="T:System.String" /> object and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified string. </summary>
        /// <param name="strB">The string to compare with this instance. </param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="strB" /> parameter.Value Condition Less than zero This instance precedes <paramref name="strB" />. Zero This instance has the same position in the sort order as <paramref name="strB" />. Greater than zero This instance follows <paramref name="strB" />.-or-
        /// <paramref name="strB" /> is <see langword="null" />. </returns>
        public int CompareTo(string strB) {
            return String.Compare(_value, strB, StringComparison.Ordinal);
        }

        /// <summary>Compares this instance with a specified <see cref="T:System.Object" /> and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified <see cref="T:System.Object" />.</summary>
        /// <param name="value">An object that evaluates to a <see cref="T:System.String" />. </param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="value" /> parameter.Value Condition Less than zero This instance precedes <paramref name="value" />. Zero This instance has the same position in the sort order as <paramref name="value" />. Greater than zero This instance follows <paramref name="value" />.-or-
        /// <paramref name="value" /> is <see langword="null" />. </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="value" /> is not a <see cref="T:System.String" />. </exception>
        public int CompareTo(object value) {
            return _value.CompareTo(value);
        }

        /// <summary>Returns a value indicating whether a specified substring occurs within this string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter occurs within this string, or if <paramref name="value" /> is the empty string (""); otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public bool Contains(string value) {
            return _value.Contains(value);
        }

        /// <summary>Copies a specified number of characters from a specified position in this instance to a specified position in an array of Unicode characters.</summary>
        /// <param name="sourceIndex">The index of the first character in this instance to copy. </param>
        /// <param name="destination">An array of Unicode characters to which characters in this instance are copied. </param>
        /// <param name="destinationIndex">The index in <paramref name="destination" /> at which the copy operation begins. </param>
        /// <param name="count">The number of characters in this instance to copy to <paramref name="destination" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destination" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex" />, <paramref name="destinationIndex" />, or <paramref name="count" /> is negative -or-
        /// <paramref name="sourceIndex" /> does not identify a position in the current instance. -or-
        /// <paramref name="destinationIndex" /> does not identify a valid index in the <paramref name="destination" /> array. -or-
        /// <paramref name="count" /> is greater than the length of the substring from <paramref name="startIndex" /> to the end of this instance -or-
        /// <paramref name="count" /> is greater than the length of the subarray from <paramref name="destinationIndex" /> to the end of the <paramref name="destination" /> array. </exception>
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) {
            _value.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        /// <summary>Determines whether the end of this string instance matches the specified string.</summary>
        /// <param name="value">The string to compare to the substring at the end of this instance. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value" /> matches the end of this instance; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public bool EndsWith(string value) {
            return _value.EndsWith(value);
        }

        /// <summary>Determines whether the end of this string instance matches the specified string when compared using the specified comparison option.</summary>
        /// <param name="value">The string to compare to the substring at the end of this instance. </param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value" /> are compared. </param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter matches the end of this string; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
        public bool EndsWith(string value, StringComparison comparisonType) {
            return _value.EndsWith(value, comparisonType);
        }

        /// <summary>Determines whether the end of this string instance matches the specified string when compared using the specified culture.</summary>
        /// <param name="value">The string to compare to the substring at the end of this instance. </param>
        /// <param name="ignoreCase">
        /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
        /// <param name="culture">Cultural information that determines how this instance and <paramref name="value" /> are compared. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter matches the end of this string; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public bool EndsWith(string value, bool ignoreCase, CultureInfo culture) {
            return _value.EndsWith(value, ignoreCase, culture);
        }

        /// <summary>Determines whether this instance and another specified <see cref="T:System.String" /> object have the same value.</summary>
        /// <param name="value">The string to compare to this instance. </param>
        /// <returns>
        /// <see langword="true" /> if the value of the <paramref name="value" /> parameter is the same as the value of this instance; otherwise, <see langword="false" />. If <paramref name="value" /> is <see langword="null" />, the method returns <see langword="false" />. </returns>
        public bool Equals(string value) {
            return _value.Equals(value);
        }

        /// <summary>Determines whether this string and a specified <see cref="T:System.String" /> object have the same value. A parameter specifies the culture, case, and sort rules used in the comparison.</summary>
        /// <param name="value">The string to compare to this instance.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared. </param>
        /// <returns>
        /// <see langword="true" /> if the value of the <paramref name="value" /> parameter is the same as this string; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value. </exception>
        public bool Equals(string value, StringComparison comparisonType) {
            return _value.Equals(value, comparisonType);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) _value).GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<char> IEnumerable<char>.GetEnumerator() {
            return _value.GetEnumerator();
        }

        /// <summary>Retrieves an object that can iterate through the individual characters in this string.</summary>
        /// <returns>An enumerator object.</returns>
        public CharEnumerator GetEnumerator() {
            return _value.GetEnumerator();
        }

        /// <summary>Returns the <see cref="T:System.TypeCode" /> for class <see cref="T:System.String" />.</summary>
        /// <returns>The enumerated constant, <see cref="F:System.TypeCode.String" />.</returns>
        public TypeCode GetTypeCode() {
            return _value.GetTypeCode();
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified Unicode character in this string.</summary>
        /// <param name="value">A Unicode character to seek. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not.</returns>
        public int IndexOf(char value) {
            return _value.IndexOf(value);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified Unicode character in this string. The search starts at a specified character position.</summary>
        /// <param name="value">A Unicode character to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> from the start of the string if that character is found, or -1 if it is not.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than 0 (zero) or greater than the length of the string. </exception>
        public int IndexOf(char value, int startIndex) {
            return _value.IndexOf(value, startIndex);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance.</summary>
        /// <param name="value">The string to seek. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is 0.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public int IndexOf(string value) {
            return _value.IndexOf(value);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than 0 (zero) or greater than the length of this string.</exception>
        public int IndexOf(string value, int startIndex) {
            return _value.IndexOf(value, startIndex);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position and examines a specified number of character positions.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> or <paramref name="startIndex" /> is negative.-or-
        /// <paramref name="startIndex" /> is greater than the length of this string.-or-
        /// <paramref name="count" /> is greater than the length of this string minus <paramref name="startIndex" />.</exception>
        public int IndexOf(string value, int startIndex, int count) {
            return _value.IndexOf(value, startIndex, count);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. A parameter specifies the type of search to use for the specified string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>The index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is 0.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
        public int IndexOf(string value, StringComparison comparisonType) {
            return _value.IndexOf(value, comparisonType);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. Parameters specify the starting search position in the current string and the type of search to use for the specified string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>The zero-based index position of the <paramref name="value" /> parameter from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than 0 (zero) or greater than the length of this string. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType) {
            return _value.IndexOf(value, startIndex, comparisonType);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified character in this instance. The search starts at a specified character position and examines a specified number of character positions.</summary>
        /// <param name="value">A Unicode character to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> from the start of the string if that character is found, or -1 if it is not.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> or <paramref name="startIndex" /> is negative.-or-
        /// <paramref name="startIndex" /> is greater than the length of this string.-or-
        /// <paramref name="count" /> is greater than the length of this string minus <paramref name="startIndex" />.</exception>
        public int IndexOf(char value, int startIndex, int count) {
            return _value.IndexOf(value, startIndex, count);
        }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. Parameters specify the starting search position in the current string, the number of characters in the current string to search, and the type of search to use for the specified string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>The zero-based index position of the <paramref name="value" /> parameter from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> or <paramref name="startIndex" /> is negative.-or-
        /// <paramref name="startIndex" /> is greater than the length of this instance.-or-
        /// <paramref name="count" /> is greater than the length of this string minus <paramref name="startIndex" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
        public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType) {
            return _value.IndexOf(value, startIndex, count, comparisonType);
        }

        /// <summary>Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters. The search starts at a specified character position and examines a specified number of character positions.</summary>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <returns>The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="anyOf" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> or <paramref name="startIndex" /> is negative.-or-
        /// <paramref name="count" /> + <paramref name="startIndex" /> is greater than the number of characters in this instance. </exception>
        public int IndexOfAny(char[] anyOf, int startIndex, int count) {
            return _value.IndexOfAny(anyOf, startIndex, count);
        }

        /// <summary>Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters. The search starts at a specified character position.</summary>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek. </param>
        /// <param name="startIndex">The search starting position. </param>
        /// <returns>The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="anyOf" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is negative.-or-
        /// <paramref name="startIndex" /> is greater than the number of characters in this instance. </exception>
        public int IndexOfAny(char[] anyOf, int startIndex) {
            return _value.IndexOfAny(anyOf, startIndex);
        }

        /// <summary>Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters.</summary>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek. </param>
        /// <returns>The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="anyOf" /> is <see langword="null" />. </exception>
        public int IndexOfAny(char[] anyOf) {
            return _value.IndexOfAny(anyOf);
        }

        /// <summary>
        /// Returns a new string in which a specified string is inserted at a specified index position in this instance.</summary>
        /// <param name="startIndex">The zero-based index position of the insertion. </param>
        /// <param name="value">The string to insert. </param>
        /// <returns>A new string that is equivalent to this instance, but with <paramref name="value" /> inserted at position <paramref name="startIndex" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is negative or greater than the length of this instance. </exception>
        public string Insert(int startIndex, string value) {
            return _value.Insert(startIndex, value);
        }

        /// <summary>Indicates whether this string is in Unicode normalization form C.</summary>
        /// <returns>
        /// <see langword="true" /> if this string is in normalization form C; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
        public bool IsNormalized() {
            return _value.IsNormalized();
        }

        /// <summary>Indicates whether this string is in the specified Unicode normalization form.</summary>
        /// <param name="normalizationForm">A Unicode normalization form. </param>
        /// <returns>
        /// <see langword="true" /> if this string is in the normalization form specified by the <paramref name="normalizationForm" /> parameter; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
        public bool IsNormalized(NormalizationForm normalizationForm) {
            return _value.IsNormalized(normalizationForm);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of a specified Unicode character within this instance.</summary>
        /// <param name="value">The Unicode character to seek. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not.</returns>
        public int LastIndexOf(char value) {
            return _value.LastIndexOf(value);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for the specified number of character positions. A parameter specifies the type of comparison to perform when searching for the specified string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>The zero-based starting index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the smaller of <paramref name="startIndex" /> and the last index position in this instance.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is negative.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is negative.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is greater than the length of this instance.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> + 1 - <paramref name="count" /> specifies a position that is not within this instance. -or-The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="start" /> is less than -1 or greater than zero. -or-The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="count" /> is greater than 1. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
        public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType) {
            return _value.LastIndexOf(value, startIndex, count, comparisonType);
        }

        /// <summary>Reports the zero-based index of the last occurrence of a specified string within the current <see cref="T:System.String" /> object. The search starts at a specified character position and proceeds backward toward the beginning of the string. A parameter specifies the type of comparison to perform when searching for the specified string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>The zero-based starting index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the smaller of <paramref name="startIndex" /> and the last index position in this instance.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than the length of the current instance. -or-The current instance equals <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than -1 or greater than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType) {
            return _value.LastIndexOf(value, startIndex, comparisonType);
        }

        /// <summary>Reports the zero-based index of the last occurrence of a specified string within the current <see cref="T:System.String" /> object. A parameter specifies the type of search to use for the specified string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>The zero-based starting index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the last index position in this instance.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
        public int LastIndexOf(string value, StringComparison comparisonType) {
            return _value.LastIndexOf(value, comparisonType);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <returns>The zero-based starting index position of <paramref name="value" /> if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the smaller of <paramref name="startIndex" /> and the last index position in this instance.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is negative.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is negative.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is greater than the length of this instance.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> - <paramref name="count" />+ 1 specifies a position that is not within this instance. -or-The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="start" /> is less than -1 or greater than zero. -or-The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="count" /> is greater than 1. </exception>
        public int LastIndexOf(string value, int startIndex, int count) {
            return _value.LastIndexOf(value, startIndex, count);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.</summary>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <returns>The zero-based starting index position of <paramref name="value" /> if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the smaller of <paramref name="startIndex" /> and the last index position in this instance.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than the length of the current instance. -or-The current instance equals <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than -1 or greater than zero.</exception>
        public int LastIndexOf(string value, int startIndex) {
            return _value.LastIndexOf(value, startIndex);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of the specified Unicode character in a substring within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.</summary>
        /// <param name="value">The Unicode character to seek. </param>
        /// <param name="startIndex">The starting position of the search. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than or equal to the length of this instance.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> - <paramref name="count" /> + 1 is less than zero.</exception>
        public int LastIndexOf(char value, int startIndex, int count) {
            return _value.LastIndexOf(value, startIndex, count);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance.</summary>
        /// <param name="value">The string to seek. </param>
        /// <returns>The zero-based starting index position of <paramref name="value" /> if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the last index position in this instance.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public int LastIndexOf(string value) {
            return _value.LastIndexOf(value);
        }

        /// <summary>Reports the zero-based index position of the last occurrence of a specified Unicode character within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.</summary>
        /// <param name="value">The Unicode character to seek. </param>
        /// <param name="startIndex">The starting position of the search. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than or equal to the length of this instance.</exception>
        public int LastIndexOf(char value, int startIndex) {
            return _value.LastIndexOf(value, startIndex);
        }

        /// <summary>Reports the zero-based index position of the last occurrence in this instance of one or more characters specified in a Unicode array. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.</summary>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek. </param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <param name="count">The number of character positions to examine. </param>
        /// <returns>The index position of the last occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="anyOf" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="count" /> or <paramref name="startIndex" /> is negative.-or-
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> minus <paramref name="count" /> + 1 is less than zero. </exception>
        public int LastIndexOfAny(char[] anyOf, int startIndex, int count) {
            return _value.LastIndexOfAny(anyOf, startIndex, count);
        }

        /// <summary>Reports the zero-based index position of the last occurrence in this instance of one or more characters specified in a Unicode array. The search starts at a specified character position and proceeds backward toward the beginning of the string.</summary>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek. </param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
        /// <returns>The index position of the last occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="anyOf" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> specifies a position that is not within this instance. </exception>
        public int LastIndexOfAny(char[] anyOf, int startIndex) {
            return _value.LastIndexOfAny(anyOf, startIndex);
        }

        /// <summary>Reports the zero-based index position of the last occurrence in this instance of one or more characters specified in a Unicode array.</summary>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek. </param>
        /// <returns>The index position of the last occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="anyOf" /> is <see langword="null" />. </exception>
        public int LastIndexOfAny(char[] anyOf) {
            return _value.LastIndexOfAny(anyOf);
        }

        /// <summary>Gets the number of characters in the current <see cref="T:System.String" /> object.</summary>
        /// <returns>The number of characters in the current string.</returns>
        public int Length => _value.Length;

        /// <summary>Returns a new string whose textual value is the same as this string, but whose binary representation is in Unicode normalization form C.</summary>
        /// <returns>A new, normalized string whose textual value is the same as this string, but whose binary representation is in normalization form C.</returns>
        /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
        public string Normalize() {
            return _value.Normalize();
        }

        /// <summary>Returns a new string whose textual value is the same as this string, but whose binary representation is in the specified Unicode normalization form.</summary>
        /// <param name="normalizationForm">A Unicode normalization form. </param>
        /// <returns>A new string whose textual value is the same as this string, but whose binary representation is in the normalization form specified by the <paramref name="normalizationForm" /> parameter.</returns>
        /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
        public string Normalize(NormalizationForm normalizationForm) {
            return _value.Normalize(normalizationForm);
        }

        /// <summary>Returns a new string that right-aligns the characters in this instance by padding them on the left with a specified Unicode character, for a specified total length.</summary>
        /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. </param>
        /// <param name="paddingChar">A Unicode padding character. </param>
        /// <returns>A new string that is equivalent to this instance, but right-aligned and padded on the left with as many <paramref name="paddingChar" /> characters as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="totalWidth" /> is less than zero. </exception>
        public string PadLeft(int totalWidth, char paddingChar) {
            return _value.PadLeft(totalWidth, paddingChar);
        }

        /// <summary>Returns a new string that right-aligns the characters in this instance by padding them with spaces on the left, for a specified total length.</summary>
        /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. </param>
        /// <returns>A new string that is equivalent to this instance, but right-aligned and padded on the left with as many spaces as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="totalWidth" /> is less than zero. </exception>
        public string PadLeft(int totalWidth) {
            return _value.PadLeft(totalWidth);
        }

        /// <summary>Returns a new string that left-aligns the characters in this string by padding them with spaces on the right, for a specified total length.</summary>
        /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. </param>
        /// <returns>A new string that is equivalent to this instance, but left-aligned and padded on the right with as many spaces as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="totalWidth" /> is less than zero. </exception>
        public string PadRight(int totalWidth) {
            return _value.PadRight(totalWidth);
        }

        /// <summary>Returns a new string that left-aligns the characters in this string by padding them on the right with a specified Unicode character, for a specified total length.</summary>
        /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. </param>
        /// <param name="paddingChar">A Unicode padding character. </param>
        /// <returns>A new string that is equivalent to this instance, but left-aligned and padded on the right with as many <paramref name="paddingChar" /> characters as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="totalWidth" /> is less than zero. </exception>
        public string PadRight(int totalWidth, char paddingChar) {
            return _value.PadRight(totalWidth, paddingChar);
        }

        /// <summary>
        /// Returns a new string in which all the characters in the current instance, beginning at a specified position and continuing through the last position, have been deleted.</summary>
        /// <param name="startIndex">The zero-based position to begin deleting characters. </param>
        /// <returns>A new string that is equivalent to this string except for the removed characters.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than zero.-or-
        /// <paramref name="startIndex" /> specifies a position that is not within this string. </exception>
        public string Remove(int startIndex) {
            return _value.Remove(startIndex);
        }

        /// <summary>
        /// Returns a new string in which a specified number of characters in the current instance beginning at a specified position have been deleted.</summary>
        /// <param name="startIndex">The zero-based position to begin deleting characters. </param>
        /// <param name="count">The number of characters to delete. </param>
        /// <returns>A new string that is equivalent to this instance except for the removed characters.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Either <paramref name="startIndex" /> or <paramref name="count" /> is less than zero.-or-
        /// <paramref name="startIndex" /> plus <paramref name="count" /> specify a position outside this instance. </exception>
        public string Remove(int startIndex, int count) {
            return _value.Remove(startIndex, count);
        }

        /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.</summary>
        /// <param name="oldValue">The string to be replaced. </param>
        /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue" />. </param>
        /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with <paramref name="newValue" />. If <paramref name="oldValue" /> is not found in the current instance, the method returns the current instance unchanged. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="oldValue" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="oldValue" /> is the empty string (""). </exception>
        public string Replace(string oldValue, string newValue) {
            return _value.Replace(oldValue, newValue);
        }

        /// <summary>Returns a new string in which all occurrences of a specified Unicode character in this instance are replaced with another specified Unicode character.</summary>
        /// <param name="oldChar">The Unicode character to be replaced. </param>
        /// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChar" />. </param>
        /// <returns>A string that is equivalent to this instance except that all instances of <paramref name="oldChar" /> are replaced with <paramref name="newChar" />. If <paramref name="oldChar" /> is not found in the current instance, the method returns the current instance unchanged. </returns>
        public string Replace(char oldChar, char newChar) {
            return _value.Replace(oldChar, newChar);
        }

        /// <summary>Splits a string into a maximum number of substrings based on the strings in an array. You can specify whether the substrings include empty array elements.</summary>
        /// <param name="separator">A string array that delimits the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />. </param>
        /// <param name="count">The maximum number of substrings to return. </param>
        /// <param name="options">
        /// <see cref="F:System.StringSplitOptions.RemoveEmptyEntries" /> to omit empty array elements from the array returned; or <see cref="F:System.StringSplitOptions.None" /> to include empty array elements in the array returned. </param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more strings in <paramref name="separator" />. For more information, see the Remarks section.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is negative. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
        public string[] Split(string[] separator, int count, StringSplitOptions options) {
            return _value.Split(separator, count, options);
        }

        /// <summary>Splits a string into substrings based on the strings in an array. You can specify whether the substrings include empty array elements.</summary>
        /// <param name="separator">A string array that delimits the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />. </param>
        /// <param name="options">
        /// <see cref="F:System.StringSplitOptions.RemoveEmptyEntries" /> to omit empty array elements from the array returned; or <see cref="F:System.StringSplitOptions.None" /> to include empty array elements in the array returned. </param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more strings in <paramref name="separator" />. For more information, see the Remarks section.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
        public string[] Split(string[] separator, StringSplitOptions options) {
            return _value.Split(separator, options);
        }

        /// <summary>Splits a string into a maximum number of substrings based on the characters in an array.</summary>
        /// <param name="separator">A character array that delimits the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />. </param>
        /// <param name="count">The maximum number of substrings to return. </param>
        /// <param name="options">
        /// <see cref="F:System.StringSplitOptions.RemoveEmptyEntries" /> to omit empty array elements from the array returned; or <see cref="F:System.StringSplitOptions.None" /> to include empty array elements in the array returned. </param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is negative. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
        public string[] Split(char[] separator, int count, StringSplitOptions options) {
            return _value.Split(separator, count, options);
        }

        /// <summary>Splits a string into substrings based on the characters in an array. You can specify whether the substrings include empty array elements.</summary>
        /// <param name="separator">A character array that delimits the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />. </param>
        /// <param name="options">
        /// <see cref="F:System.StringSplitOptions.RemoveEmptyEntries" /> to omit empty array elements from the array returned; or <see cref="F:System.StringSplitOptions.None" /> to include empty array elements in the array returned. </param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
        public string[] Split(char[] separator, StringSplitOptions options) {
            return _value.Split(separator, options);
        }

        /// <summary>Splits a string into a maximum number of substrings based on the characters in an array. You also specify the maximum number of substrings to return.</summary>
        /// <param name="separator">A character array that delimits the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />. </param>
        /// <param name="count">The maximum number of substrings to return. </param>
        /// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is negative. </exception>
        public string[] Split(char[] separator, int count) {
            return _value.Split(separator, count);
        }

        /// <summary>Splits a string into substrings that are based on the characters in an array. </summary>
        /// <param name="separator">A character array that delimits the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />. </param>
        /// <returns>An array whose elements contain the substrings from this instance that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
        public string[] Split(params char[] separator) {
            return _value.Split(separator);
        }

        /// <summary>Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.</summary>
        /// <param name="value">The string to compare. </param>
        /// <param name="ignoreCase">
        /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
        /// <param name="culture">Cultural information that determines how this string and <paramref name="value" /> are compared. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter matches the beginning of this string; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public bool StartsWith(string value, bool ignoreCase, CultureInfo culture) {
            return _value.StartsWith(value, ignoreCase, culture);
        }

        /// <summary>Determines whether the beginning of this string instance matches the specified string when compared using the specified comparison option.</summary>
        /// <param name="value">The string to compare. </param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value" /> are compared. </param>
        /// <returns>
        /// <see langword="true" /> if this instance begins with <paramref name="value" />; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
        public bool StartsWith(string value, StringComparison comparisonType) {
            return _value.StartsWith(value, comparisonType);
        }

        /// <summary>Determines whether the beginning of this string instance matches the specified string.</summary>
        /// <param name="value">The string to compare. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value" /> matches the beginning of this string; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />. </exception>
        public bool StartsWith(string value) {
            return _value.StartsWith(value);
        }

        /// <summary>Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.</summary>
        /// <param name="startIndex">The zero-based starting character position of a substring in this instance. </param>
        /// <param name="length">The number of characters in the substring. </param>
        /// <returns>A string that is equivalent to the substring of length <paramref name="length" /> that begins at <paramref name="startIndex" /> in this instance, or <see cref="F:System.String.Empty" /> if <paramref name="startIndex" /> is equal to the length of this instance and <paramref name="length" /> is zero.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> plus <paramref name="length" /> indicates a position not within this instance.-or-
        /// <paramref name="startIndex" /> or <paramref name="length" /> is less than zero. </exception>
        public string Substring(int startIndex, int length) {
            try {
                if (startIndex < 0)
                    return "";
                var str = base.Value as string;
                if (startIndex >= str.Length)
                    return "";

                return str.Substring(startIndex, Math.Min(length, str.Length - startIndex));
            } catch (IndexOutOfRangeException) {
                return "";
            }
        }

        /// <summary>Retrieves a substring from this instance. The substring starts at a specified character position and continues to the end of the string.</summary>
        /// <param name="startIndex">The zero-based starting character position of a substring in this instance. </param>
        /// <returns>A string that is equivalent to the substring that begins at <paramref name="startIndex" /> in this instance, or <see cref="F:System.String.Empty" /> if <paramref name="startIndex" /> is equal to the length of this instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than zero or greater than the length of this instance. </exception>
        public string Substring(int startIndex) {
            return Substring(startIndex, Length - startIndex);
        }

        /// <summary>Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        public bool ToBoolean(IFormatProvider provider) {
            return ((IConvertible) _value).ToBoolean(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        public byte ToByte(IFormatProvider provider) {
            return ((IConvertible) _value).ToByte(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        public char ToChar(IFormatProvider provider) {
            return ((IConvertible) _value).ToChar(provider);
        }

        /// <summary>Copies the characters in a specified substring in this instance to a Unicode character array.</summary>
        /// <param name="startIndex">The starting position of a substring in this instance. </param>
        /// <param name="length">The length of the substring in this instance. </param>
        /// <returns>A Unicode character array whose elements are the <paramref name="length" /> number of characters in this instance starting from character position <paramref name="startIndex" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.-or-
        /// <paramref name="startIndex" /> plus <paramref name="length" /> is greater than the length of this instance. </exception>
        public char[] ToCharArray(int startIndex, int length) {
            return _value.ToCharArray(startIndex, length);
        }

        /// <summary>Copies the characters in this instance to a Unicode character array. </summary>
        /// <returns>A Unicode character array whose elements are the individual characters of this instance. If this instance is an empty string, the returned array is empty and has a zero length.</returns>
        public char[] ToCharArray() {
            return _value.ToCharArray();
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.DateTime" /> using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A <see cref="T:System.DateTime" /> instance equivalent to the value of this instance.</returns>
        public DateTime ToDateTime(IFormatProvider provider) {
            return ((IConvertible) _value).ToDateTime(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.Decimal" /> number using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A <see cref="T:System.Decimal" /> number equivalent to the value of this instance.</returns>
        public decimal ToDecimal(IFormatProvider provider) {
            return ((IConvertible) _value).ToDecimal(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        public double ToDouble(IFormatProvider provider) {
            return ((IConvertible) _value).ToDouble(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        public short ToInt16(IFormatProvider provider) {
            return ((IConvertible) _value).ToInt16(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        public int ToInt32(IFormatProvider provider) {
            return ((IConvertible) _value).ToInt32(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        public long ToInt64(IFormatProvider provider) {
            return ((IConvertible) _value).ToInt64(provider);
        }

        /// <summary>Returns a copy of this string converted to lowercase, using the casing rules of the specified culture.</summary>
        /// <param name="culture">An object that supplies culture-specific casing rules. </param>
        /// <returns>The lowercase equivalent of the current string.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="culture" /> is <see langword="null" />. </exception>
        public string ToLower(CultureInfo culture) {
            return _value.ToLower(culture);
        }

        /// <summary>Returns a copy of this string converted to lowercase.</summary>
        /// <returns>A string in lowercase.</returns>
        public string ToLower() {
            return _value.ToLower();
        }

        /// <summary>Returns a copy of this <see cref="T:System.String" /> object converted to lowercase using the casing rules of the invariant culture.</summary>
        /// <returns>The lowercase equivalent of the current string.</returns>
        public string ToLowerInvariant() {
            return _value.ToLowerInvariant();
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        public sbyte ToSByte(IFormatProvider provider) {
            return ((IConvertible) _value).ToSByte(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        public float ToSingle(IFormatProvider provider) {
            return ((IConvertible) _value).ToSingle(provider);
        }

        /// <summary>Returns this instance of <see cref="T:System.String" />; no actual conversion is performed.</summary>
        /// <param name="provider">(Reserved) An object that supplies culture-specific formatting information. </param>
        /// <returns>The current string.</returns>
        public string ToString(IFormatProvider provider) {
            return _value.ToString(provider);
        }

        /// <summary>Converts the value of this instance to an <see cref="T:System.Object" /> of the specified <see cref="T:System.Type" /> that has an equivalent value, using the specified culture-specific formatting information.</summary>
        /// <param name="conversionType">The <see cref="T:System.Type" /> to which the value of this instance is converted. </param>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An <see cref="T:System.Object" /> instance of type <paramref name="conversionType" /> whose value is equivalent to the value of this instance.</returns>
        public object ToType(Type conversionType, IFormatProvider provider) {
            return ((IConvertible) _value).ToType(conversionType, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        public ushort ToUInt16(IFormatProvider provider) {
            return ((IConvertible) _value).ToUInt16(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        public uint ToUInt32(IFormatProvider provider) {
            return ((IConvertible) _value).ToUInt32(provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        public ulong ToUInt64(IFormatProvider provider) {
            return ((IConvertible) _value).ToUInt64(provider);
        }

        /// <summary>Returns a copy of this string converted to uppercase.</summary>
        /// <returns>The uppercase equivalent of the current string.</returns>
        public string ToUpper() {
            return _value.ToUpper();
        }

        /// <summary>Returns a copy of this string converted to uppercase, using the casing rules of the specified culture.</summary>
        /// <param name="culture">An object that supplies culture-specific casing rules. </param>
        /// <returns>The uppercase equivalent of the current string.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="culture" /> is <see langword="null" />. </exception>
        public string ToUpper(CultureInfo culture) {
            return _value.ToUpper(culture);
        }

        /// <summary>Returns a copy of this <see cref="T:System.String" /> object converted to uppercase using the casing rules of the invariant culture.</summary>
        /// <returns>The uppercase equivalent of the current string.</returns>
        public string ToUpperInvariant() {
            return _value.ToUpperInvariant();
        }

        /// <summary>Removes all leading and trailing white-space characters from the current <see cref="T:System.String" /> object.</summary>
        /// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged. </returns>
        public string Trim() {
            return _value.Trim();
        }

        /// <summary>Removes all leading and trailing occurrences of a set of characters specified in an array from the current <see cref="T:System.String" /> object.</summary>
        /// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null" />. </param>
        /// <returns>The string that remains after all occurrences of the characters in the <paramref name="trimChars" /> parameter are removed from the start and end of the current string. If <paramref name="trimChars" /> is <see langword="null" /> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
        public string Trim(params char[] trimChars) {
            return _value.Trim(trimChars);
        }

        /// <summary>Removes all trailing occurrences of a set of characters specified in an array from the current <see cref="T:System.String" /> object.</summary>
        /// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null" />. </param>
        /// <returns>The string that remains after all occurrences of the characters in the <paramref name="trimChars" /> parameter are removed from the end of the current string. If <paramref name="trimChars" /> is <see langword="null" /> or an empty array, Unicode white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged. </returns>
        public string TrimEnd(params char[] trimChars) {
            return _value.TrimEnd(trimChars);
        }

        /// <summary>Removes all leading occurrences of a set of characters specified in an array from the current <see cref="T:System.String" /> object.</summary>
        /// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null" />. </param>
        /// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars" /> parameter are removed from the start of the current string. If <paramref name="trimChars" /> is <see langword="null" /> or an empty array, white-space characters are removed instead.</returns>
        public string TrimStart(params char[] trimChars) {
            return _value.TrimStart(trimChars);
        }

        #endregion

        #region Equality


        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(StringScalar other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(Value, other.Value as string);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringScalar) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.StringScalar" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(StringScalar left, StringScalar right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.StringScalar" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(StringScalar left, StringScalar right) {
            return !Equals(left, right);
        }

        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return _value;
        }
    }
}