using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler.Helpers;

namespace Regen.Core.Tests {
    [TestClass, Ignore("StringSpan is not used")]
    public class StringSpanTests {
        [TestMethod]
        public void Range_Sub() {
            Range left;
            Range right;
            Range result;

            //contained
            left = new Range(0, 9);
            right = new Range(3, 6);
            result = Range.Subtract(left, right);
            result.Start.Should().Be(0);
            result.End.Should().Be(5);

            left = new Range(10, 19);
            right = new Range(13, 16);
            result = Range.Subtract(left, right);
            result.Start.Should().Be(10);
            result.End.Should().Be(15);

            //above equals
            left = new Range(10, 19);
            right = new Range(19, 29);
            result = Range.Subtract(left, right);
            result.Start.Should().Be(10);
            result.End.Should().Be(18);

            //above
            left = new Range(10, 19);
            right = new Range(20, 29);
            result = Range.Subtract(left, right);
            result.Should().BeEquivalentTo(left);

            //above partial
            left = new Range(10, 19);
            right = new Range(15, 29);
            result = Range.Subtract(left, right);
            result.Start.Should().Be(10);
            result.End.Should().Be(14);

            //below partial
            left = new Range(10, 19);
            right = new Range(5, 15);
            result = Range.Subtract(left, right);
            result.Start.Should().Be(5);
            result.End.Should().Be(9);
        }

        [TestMethod]
        public void SpanString_Remove() {
            StringSource spanner;
            StringSlice sub;

            //contains
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.RemoveAt(1);
            sub.ToString().Should().Be("hy");

            //swallow
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.Remove(new Range(0, 2));
            sub.ToString().Should().Be("");
            sub.Deleted.Should().BeTrue();

            //lower partial
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(5, 6));
            spanner.Remove(new Range(5, 5));
            sub.ToString().Should().Be("e");
            sub.Deleted.Should().BeFalse();

            //lower equal bound
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.Remove(new Range(2, 3));
            sub.ToString().Should().Be("he");
            sub.Deleted.Should().BeFalse();

            //above full
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.Remove(new Range(3, 8));
            sub.ToString().Should().Be("hey");
            spanner.ToString().Should().Be("hey");
            sub.Deleted.Should().BeFalse();

            //above partially
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.Remove(new Range(2, 8));
            sub.ToString().Should().Be("he");
            spanner.ToString().Should().Be("he");
            sub.Deleted.Should().BeFalse();
        }

        [TestMethod]
        public void SpanString_Insert() {
            StringSource spanner;
            StringSlice sub;

            //above
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.Remove(new Range(4, 8));
            spanner.Insert(4, "dad");
            sub.ToString().Should().Be("hey");
            spanner.ToString().Should().Be("hey dad");

            //exact
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.Remove(new Range(0, 2));
            spanner.Insert(0, "bye");
            spanner.ToString().Should().Be("bye there");
        }

        [TestMethod]
        public void SpanString_RemovePlaceAt() {
            StringSource spanner;
            StringSlice sub;

            //above
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.ExchangeAt(4, 8, "dad");
            sub.ToString().Should().Be("hey");
            spanner.ToString().Should().Be("hey dad");

            //exact
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            spanner.ExchangeAt(0, 2, "bye");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("bye");
            spanner.ToString().Should().Be("bye there");

            //exact
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(1, 2));
            spanner.ExchangeAt(0, 2, "bye");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("ye");
            spanner.ToString().Should().Be("bye there");

            //exact
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(1, 2));
            spanner.ExchangeAt(0, 2, "bye");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("ye");
            spanner.ToString().Should().Be("bye there");

            //exact
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(4, 8));
            spanner.ExchangeAt(0, 3, "bye");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            spanner.ToString().Should().Be("byethere");

            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(4, 8));
            spanner.ExchangeAt(4, 8, "cowboy");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("cowboy");
            spanner.ToString().Should().Be("hey cowboy");
        }


        [TestMethod]
        public void SpanString_Add() {
            StringSource spanner;
            StringSlice sub;

            //Add inside a word
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(0, 2));
            sub.Add("hey");
            sub.ToString().Should().Be("heyhey");
            spanner.ToString().Should().Be("heyhey there");

            //Add at the end of a word
            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(4, 8));
            sub.Add("hey");
            sub.ToString().Should().Be("therehey");
            spanner.ToString().Should().Be("hey therehey");
            spanner.RemoveAt(3);
            spanner.ToString().Should().Be("heytherehey");
        }

        [TestMethod]
        public void TrimEnd_mutiple_symbols() {
            var spanner = new StringSource("hey there!@!");
            spanner.TrimEnd('!', '@');
            spanner.ToString().Should().Be("hey there");
        }

        [TestMethod]
        public void trimend_single_char() {
            var spanner = new StringSource("hey there!");
            spanner.TrimEnd('!');
            spanner.ToString().Should().Be("hey there");
        }

        [TestMethod]
        public void TrimEnd_nested_substring() {
            var spanner = new StringSource("hey there!!hy!!");
            var sub = spanner.Substring(new Range(11, 12));
            spanner.TrimEnd('!', 'h', 'y');
            spanner.ToString().Should().Be("hey there");
            sub.Deleted.Should().BeTrue();
        }

        [TestMethod]
        public void TrimEnd_nested_substring_no_removes() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            spanner.TrimEnd('!', 'h', 'y');
            spanner.ToString().Should().Be("hey there");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
        }

        [TestMethod]
        public void TrimStart_first_char_multiple_symbols() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            spanner.TrimStart('!', 'h', 'y');
            spanner.ToString().Should().Be("ey there");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
        }

        [TestMethod]
        public void TrimStart_word() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            var sub2 = spanner.Substring(new Range(3, 8));
            var sub3 = spanner.Substring(new Range(0, 2));
            spanner.TrimStart('h', 'e', 'y');
            spanner.ToString().Should().Be(" there");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            sub2.ToString().Should().Be(" there");
            sub3.Deleted.Should().BeTrue();
        }

        [TestMethod]
        public void TrimStart_no_matches() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            var sub2 = spanner.Substring(new Range(3, 8));
            var sub3 = spanner.Substring(new Range(0, 2));
            spanner.TrimStart(' ');
            spanner.ToString().Should().Be("hey there");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            sub2.ToString().Should().Be(" there");
            sub3.Deleted.Should().BeFalse();
            sub3.ToString().Should().Be("hey");
        }

        [TestMethod]
        public void Add_1() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            var sub2 = spanner.Substring(new Range(3, 8));
            var sub3 = spanner.Substring(new Range(0, 2));
            spanner.TrimStart(' ');
            spanner.ToString().Should().Be("hey there");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            sub2.ToString().Should().Be(" there");
            sub3.Deleted.Should().BeFalse();
            sub3.ToString().Should().Be("hey");

            spanner.Add(" boy");

            spanner.ToString().Should().Be("hey there boy");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            sub2.ToString().Should().Be(" there");
            sub3.Deleted.Should().BeFalse();
            sub3.ToString().Should().Be("hey");
        }

        [TestMethod]
        public void Add_2() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            var sub2 = spanner.Substring(new Range(3, 8));
            var sub3 = spanner.Substring(new Range(0, 2));
            spanner.TrimStart(' ');
            spanner.ToString().Should().Be("hey there");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            sub2.ToString().Should().Be(" there");
            sub3.Deleted.Should().BeFalse();
            sub3.ToString().Should().Be("hey");

            spanner.Add(" boy");

            spanner.ToString().Should().Be("hey there boy");
            sub.Deleted.Should().BeFalse();
            sub.ToString().Should().Be("there");
            sub2.ToString().Should().Be(" there");
            sub3.Deleted.Should().BeFalse();
            sub3.ToString().Should().Be("hey");
        }


        [TestMethod]
        public void substring_outofrange() {
            var spanner = new StringSource("hey there");
            new Action(() => spanner.Substring(new Range(4, 9))).Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Slice_duplicate() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            sub.Duplicate(true);
            sub.ToString().Should().Be("therethere");
            spanner.ToString().Should().Be("hey therethere");
        }

        [TestMethod]
        public void Slice_Duplicate_noappend() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(4, 8));
            var newslice = sub.Duplicate(false);
            sub.ToString().Should().Be("there");
            newslice.ToString().Should().Be("there");
            spanner.ToString().Should().Be("hey therethere");
        }

        [TestMethod]
        public void Slice_RemoveAt() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(3, 8));
            spanner.RemoveAt(3);

            sub.ToString().Should().Be("there");
            spanner.ToString().Should().Be("heythere");
        }

        [TestMethod]
        public void Slice_Remove() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(3, 8));
            spanner.Remove(3, 2);

            sub.ToString().Should().Be("here");
            spanner.ToString().Should().Be("heyhere");
        }

        [TestMethod]
        public void Slice_Trim() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(6, 8));
            sub.Trim('e');

            sub.ToString().Should().Be("r");
            spanner.ToString().Should().Be("hey thr");
        }

        [TestMethod]
        public void Spanner_Trim() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(6, 8));
            spanner.Trim('h', 'e');

            sub.ToString().Should().Be("er");
            spanner.ToString().Should().Be("y ther");
        }

        [TestMethod]
        public void Spanner_Contains() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Substring(new Range(6, 8));
            spanner.Contains("ey").Should().BeTrue();
        }

        [TestMethod]
        public void Spanner_Split() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Split('t', StringSplitOptions.None);
            sub.Should().HaveCount(2);
            sub[0].ToString().Should().Be("hey ");
            sub[1].ToString().Should().Be("here");
        }

        [TestMethod]
        public void Slice_Split() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Split('t', StringSplitOptions.None);
            sub.Should().HaveCount(2);
            sub[0].ToString().Should().Be("hey ");
            sub[1].ToString().Should().Be("here");

            var subsub = sub[1].Split('e', StringSplitOptions.RemoveEmptyEntries);
            subsub.Should().HaveCount(2);
            subsub[0].ToString().Should().Be("h");
            subsub[1].ToString().Should().Be("r");
        }

        [TestMethod]
        public void Slice_Split_Beggining() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Split('h', StringSplitOptions.None);
            sub.Should().HaveCount(2);
            sub[0].ToString().Should().Be("ey t");
            sub[1].ToString().Should().Be("ere");
        }

        [TestMethod]
        public void Slice_Split_Twice() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Split('t', StringSplitOptions.None);
            sub.Should().HaveCount(2);
            sub[0].ToString().Should().Be("hey ");
            sub[1].ToString().Should().Be("here");

            var subsub = sub[1].Split('r', StringSplitOptions.RemoveEmptyEntries);
            subsub.Should().HaveCount(2);
            subsub[0].ToString().Should().Be("he");
            subsub[1].ToString().Should().Be("e");
        }

        [TestMethod]
        public void Spanner_Split_End() {
            var spanner = new StringSource("hey there");
            var sub = spanner.Split("ther", StringSplitOptions.None);
            sub.Should().HaveCount(2);
            sub[0].ToString().Should().Be("hey ");
            sub[1].ToString().Should().Be("e");
        }

        [TestMethod]
        public void Spanner_ReplaceWith() {
            StringSource spanner;
            StringSlice sub;

            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(4, 8));
            spanner.ReplaceWith("yoo ooooo");
            sub.ToString().Should().Be("ooooo");

            spanner = new StringSource("hey there");
            sub = spanner.Substring(new Range(4, 8));
            spanner.ReplaceWith("yooooooo");
            sub.ToString().Should().Be("ooooo"); //still 5, it expends because the replacement contains this sub
        }

        [TestMethod]
        public void Spanner_IsIndexInside() {
            StringSource spanner;

            spanner = new StringSource("12345");
            spanner.IsIndexInside(-1).Should().BeFalse();
            spanner.IsIndexInside(0).Should().BeTrue();
            spanner.IsIndexInside(4).Should().BeTrue();
            spanner.IsIndexInside(5).Should().BeFalse();
        }

        [TestMethod]
        public void Slice_IsIndexInside() {
            StringSource spanner;
            StringSlice sub;

            spanner = new StringSource("12345");
            sub = spanner.Substring(1, 3);
            sub.IsIndexInside(-1).Should().BeFalse();
            sub.IsIndexInside(0).Should().BeTrue();
            sub.IsIndexInside(1).Should().BeTrue();
            sub.IsIndexInside(2).Should().BeTrue();
            sub.IsIndexInside(3).Should().BeFalse();
            sub.IsIndexInside(5).Should().BeFalse();
        }

        [TestMethod]
        public void Slice_IsIndexInside_NestedSlices() {
            StringSource spanner;
            StringSlice sub;
            StringSlice subsub;

            spanner = new StringSource("1234567890");
            sub = spanner.Substring(2, 6); //3 to 8
            sub.ToString().Should().Be("345678");
            subsub = sub.Substring(1, 4); //4 to 7
            subsub.ToString().Should().Be("4567");

            subsub.IsIndexInside(-1).Should().BeFalse();
            subsub.IsIndexInside(0).Should().BeTrue();
            subsub.IsIndexInside(1).Should().BeTrue();
            subsub.IsIndexInside(2).Should().BeTrue();
            subsub.IsIndexInside(3).Should().BeTrue();
            subsub.IsIndexInside(5).Should().BeFalse();
        }

        [TestMethod]
        public void Source_ExchangeAt() {
            StringSource spanner;
            StringSlice sub;

            //smaller exchange, large insert
            spanner = new StringSource("1234567890");
            sub = spanner.Substring(new Range(3, 6)); //3 to 8
            sub.ToString().Should().Be("4567");
            spanner.ExchangeAt(3, 6, "hello");
            sub.ToString().Should().Be("hello");
            spanner.ToString().Should().Be("123hello890")
                ;
            //smaller exchange after in sub, large insert
            spanner = new StringSource("1234567890");
            sub = spanner.Substring(new Range(3, 6)); //3 to 8
            sub.ToString().Should().Be("4567");
            spanner.ExchangeAt(6, 7, "hello");
            sub.ToString().Should().Be("456h");
            spanner.ToString().Should().Be("123456hello90");
            ;
            //behind sub
            spanner = new StringSource("1234567890");
            sub = spanner.Substring(new Range(3, 6)); //3 to 8
            sub.ToString().Should().Be("4567");
            spanner.ExchangeAt(0, 1, "hello");
            sub.ToString().Should().Be("4567");
            spanner.ToString().Should().Be("hello34567890");

            ;
            spanner = new StringSource("1234567890");
            sub = spanner.Substring(new Range(3, 6)); //3 to 8
            sub.ToString().Should().Be("4567");
            spanner.ExchangeAt(0, 5, "hell");
            sub.ToString().Should().Be("l7");
            spanner.ToString().Should().Be("hell7890");
            ;
            spanner = new StringSource("1234567890");
            sub = spanner.Substring(new Range(3, 6)); //3 to 8
            sub.ToString().Should().Be("4567");
            spanner.ExchangeAt(0, 0, "9876");
            sub.ToString().Should().Be("4567");
            spanner.ToString().Should().Be("9876234567890");
            ;
        }
    }
}