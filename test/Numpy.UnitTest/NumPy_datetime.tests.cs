// Copyright (c) 2019 by the SciSharp Team
// Code generated by CodeMinion: https://github.com/SciSharp/CodeMinion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;
using Python.Included;
using Numpy.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace Numpy.UnitTest
{
    [TestClass]
    public class NumPy_datetimeTest : BaseTestCase
    {
        
        [TestMethod]
        public void datetime_as_stringTest()
        {
            // >>> d = np.arange('2002-10-27T04:30', 4*60, 60, dtype='M8[m]')
            // >>> d
            // array(['2002-10-27T04:30', '2002-10-27T05:30', '2002-10-27T06:30',
            //        '2002-10-27T07:30'], dtype='datetime64[m]')
            // 
            
            #if TODO
            var given=  d = np.arange('2002-10-27T04:30', 4*60, 60, dtype='M8{m}');
             given=  d;
            var expected=
                "array(['2002-10-27T04:30', '2002-10-27T05:30', '2002-10-27T06:30',\n" +
                "       '2002-10-27T07:30'], dtype='datetime64[m]')";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Setting the timezone to UTC shows the same information, but with a Z suffix
            
            // >>> np.datetime_as_string(d, timezone='UTC')
            // array(['2002-10-27T04:30Z', '2002-10-27T05:30Z', '2002-10-27T06:30Z',
            //        '2002-10-27T07:30Z'], dtype='<U35')
            // 
            
            #if TODO
             given=  np.datetime_as_string(d, timezone='UTC');
             expected=
                "array(['2002-10-27T04:30Z', '2002-10-27T05:30Z', '2002-10-27T06:30Z',\n" +
                "       '2002-10-27T07:30Z'], dtype='<U35')";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Note that we picked datetimes that cross a DST boundary. Passing in a
            // pytz timezone object will print the appropriate offset
            
            // >>> np.datetime_as_string(d, timezone=pytz.timezone('US/Eastern'))
            // array(['2002-10-27T00:30-0400', '2002-10-27T01:30-0400',
            //        '2002-10-27T01:30-0500', '2002-10-27T02:30-0500'], dtype='<U39')
            // 
            
            #if TODO
             given=  np.datetime_as_string(d, timezone=pytz.timezone('US/Eastern'));
             expected=
                "array(['2002-10-27T00:30-0400', '2002-10-27T01:30-0400',\n" +
                "       '2002-10-27T01:30-0500', '2002-10-27T02:30-0500'], dtype='<U39')";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Passing in a unit will change the precision
            
            // >>> np.datetime_as_string(d, unit='h')
            // array(['2002-10-27T04', '2002-10-27T05', '2002-10-27T06', '2002-10-27T07'],
            //       dtype='<U32')
            // >>> np.datetime_as_string(d, unit='s')
            // array(['2002-10-27T04:30:00', '2002-10-27T05:30:00', '2002-10-27T06:30:00',
            //        '2002-10-27T07:30:00'], dtype='<U38')
            // 
            
            #if TODO
             given=  np.datetime_as_string(d, unit='h');
             expected=
                "array(['2002-10-27T04', '2002-10-27T05', '2002-10-27T06', '2002-10-27T07'],\n" +
                "      dtype='<U32')";
            Assert.AreEqual(expected, given.repr);
             given=  np.datetime_as_string(d, unit='s');
             expected=
                "array(['2002-10-27T04:30:00', '2002-10-27T05:30:00', '2002-10-27T06:30:00',\n" +
                "       '2002-10-27T07:30:00'], dtype='<U38')";
            Assert.AreEqual(expected, given.repr);
            #endif
            // ‘casting’ can be used to specify whether precision can be changed
            
            // >>> np.datetime_as_string(d, unit='h', casting='safe')
            // TypeError: Cannot create a datetime string as units 'h' from a NumPy
            // datetime with units 'm' according to the rule 'safe'
            // 
            
            #if TODO
             given=  np.datetime_as_string(d, unit='h', casting='safe');
             expected=
                "TypeError: Cannot create a datetime string as units 'h' from a NumPy\n" +
                "datetime with units 'm' according to the rule 'safe'";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        
        
        [TestMethod]
        public void datetime_dataTest()
        {
            // >>> dt_25s = np.dtype('timedelta64[25s]')
            // >>> np.datetime_data(dt_25s)
            // ('s', 25)
            // >>> np.array(10, dt_25s).astype('timedelta64[s]')
            // array(250, dtype='timedelta64[s]')
            // 
            
            #if TODO
            var given=  dt_25s = np.dtype('timedelta64{25s}');
             given=  np.datetime_data(dt_25s);
            var expected=
                "('s', 25)";
            Assert.AreEqual(expected, given.repr);
             given=  np.array(10, dt_25s).astype('timedelta64{s}');
             expected=
                "array(250, dtype='timedelta64[s]')";
            Assert.AreEqual(expected, given.repr);
            #endif
            // The result can be used to construct a datetime that uses the same units
            // as a timedelta
            
            // >>> np.datetime64('2010', np.datetime_data(dt_25s))
            // numpy.datetime64('2010-01-01T00:00:00', '25s')
            // 
            
            #if TODO
             given=  np.datetime64('2010', np.datetime_data(dt_25s));
             expected=
                "numpy.datetime64('2010-01-01T00:00:00', '25s')";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        
        
        [TestMethod]
        public void busdaycalendarTest()
        {
            // >>> # Some important days in July
            // ... bdd = np.busdaycalendar(
            // ...             holidays=['2011-07-01', '2011-07-04', '2011-07-17'])
            // >>> # Default is Monday to Friday weekdays
            // ... bdd.weekmask
            // array([ True,  True,  True,  True,  True, False, False], dtype='bool')
            // >>> # Any holidays already on the weekend are removed
            // ... bdd.holidays
            // array(['2011-07-01', '2011-07-04'], dtype='datetime64[D]')
            // 
            
            #if TODO
            var given=  # Some important days in July;
            var expected=
                "... bdd = np.busdaycalendar(\n" +
                "...             holidays=['2011-07-01', '2011-07-04', '2011-07-17'])";
            Assert.AreEqual(expected, given.repr);
             given=  # Default is Monday to Friday weekdays;
             expected=
                "... bdd.weekmask\n" +
                "array([ True,  True,  True,  True,  True, False, False], dtype='bool')";
            Assert.AreEqual(expected, given.repr);
             given=  # Any holidays already on the weekend are removed;
             expected=
                "... bdd.holidays\n" +
                "array(['2011-07-01', '2011-07-04'], dtype='datetime64[D]')";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        
        
        [TestMethod]
        public void is_busdayTest()
        {
            // >>> # The weekdays are Friday, Saturday, and Monday
            // ... np.is_busday(['2011-07-01', '2011-07-02', '2011-07-18'],
            // ...                 holidays=['2011-07-01', '2011-07-04', '2011-07-17'])
            // array([False, False,  True], dtype='bool')
            // 
            
            #if TODO
            var given=  # The weekdays are Friday, Saturday, and Monday;
            var expected=
                "... np.is_busday(['2011-07-01', '2011-07-02', '2011-07-18'],\n" +
                "...                 holidays=['2011-07-01', '2011-07-04', '2011-07-17'])\n" +
                "array([False, False,  True], dtype='bool')";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        
        
        [TestMethod]
        public void busday_offsetTest()
        {
            // >>> # First business day in October 2011 (not accounting for holidays)
            // ... np.busday_offset('2011-10', 0, roll='forward')
            // numpy.datetime64('2011-10-03','D')
            // >>> # Last business day in February 2012 (not accounting for holidays)
            // ... np.busday_offset('2012-03', -1, roll='forward')
            // numpy.datetime64('2012-02-29','D')
            // >>> # Third Wednesday in January 2011
            // ... np.busday_offset('2011-01', 2, roll='forward', weekmask='Wed')
            // numpy.datetime64('2011-01-19','D')
            // >>> # 2012 Mother's Day in Canada and the U.S.
            // ... np.busday_offset('2012-05', 1, roll='forward', weekmask='Sun')
            // numpy.datetime64('2012-05-13','D')
            // 
            
            #if TODO
            var given=  # First business day in October 2011 (not accounting for holidays);
            var expected=
                "... np.busday_offset('2011-10', 0, roll='forward')\n" +
                "numpy.datetime64('2011-10-03','D')";
            Assert.AreEqual(expected, given.repr);
             given=  # Last business day in February 2012 (not accounting for holidays);
             expected=
                "... np.busday_offset('2012-03', -1, roll='forward')\n" +
                "numpy.datetime64('2012-02-29','D')";
            Assert.AreEqual(expected, given.repr);
             given=  # Third Wednesday in January 2011;
             expected=
                "... np.busday_offset('2011-01', 2, roll='forward', weekmask='Wed')\n" +
                "numpy.datetime64('2011-01-19','D')";
            Assert.AreEqual(expected, given.repr);
             given=  # 2012 Mother's Day in Canada and the U.S.;
             expected=
                "... np.busday_offset('2012-05', 1, roll='forward', weekmask='Sun')\n" +
                "numpy.datetime64('2012-05-13','D')";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> # First business day on or after a date
            // ... np.busday_offset('2011-03-20', 0, roll='forward')
            // numpy.datetime64('2011-03-21','D')
            // >>> np.busday_offset('2011-03-22', 0, roll='forward')
            // numpy.datetime64('2011-03-22','D')
            // >>> # First business day after a date
            // ... np.busday_offset('2011-03-20', 1, roll='backward')
            // numpy.datetime64('2011-03-21','D')
            // >>> np.busday_offset('2011-03-22', 1, roll='backward')
            // numpy.datetime64('2011-03-23','D')
            // 
            
            #if TODO
             given=  # First business day on or after a date;
             expected=
                "... np.busday_offset('2011-03-20', 0, roll='forward')\n" +
                "numpy.datetime64('2011-03-21','D')";
            Assert.AreEqual(expected, given.repr);
             given=  np.busday_offset('2011-03-22', 0, roll='forward');
             expected=
                "numpy.datetime64('2011-03-22','D')";
            Assert.AreEqual(expected, given.repr);
             given=  # First business day after a date;
             expected=
                "... np.busday_offset('2011-03-20', 1, roll='backward')\n" +
                "numpy.datetime64('2011-03-21','D')";
            Assert.AreEqual(expected, given.repr);
             given=  np.busday_offset('2011-03-22', 1, roll='backward');
             expected=
                "numpy.datetime64('2011-03-23','D')";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        
        
        [TestMethod]
        public void busday_countTest()
        {
            // >>> # Number of weekdays in January 2011
            // ... np.busday_count('2011-01', '2011-02')
            // 21
            // >>> # Number of weekdays in 2011
            // ...  np.busday_count('2011', '2012')
            // 260
            // >>> # Number of Saturdays in 2011
            // ... np.busday_count('2011', '2012', weekmask='Sat')
            // 53
            // 
            
            #if TODO
            var given=  # Number of weekdays in January 2011;
            var expected=
                "... np.busday_count('2011-01', '2011-02')\n" +
                "21";
            Assert.AreEqual(expected, given.repr);
             given=  # Number of weekdays in 2011;
             expected=
                "...  np.busday_count('2011', '2012')\n" +
                "260";
            Assert.AreEqual(expected, given.repr);
             given=  # Number of Saturdays in 2011;
             expected=
                "... np.busday_count('2011', '2012', weekmask='Sat')\n" +
                "53";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        
    }
}
