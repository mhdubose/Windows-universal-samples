//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using Windows.Foundation.Diagnostics;

namespace LoggingCS
{
    /// <summary>
    /// Demonstrate usage of LoggingChannel and LoggingActivity.
    /// </summary>
    internal class LoggingChannelScenario
    {
        /// <summary>
        /// Construct a LoggingChannel with the Windows 8.1 constructor, then
        /// use the LoggingChannel for the scenario.
        /// </summary>
        public void LogWithWin81Constructor()
        {
            /*
            You can collect the events generated by this method with xperf or another
            ETL controller tool. To collect these events in an ETL file:

            xperf -start MySession -f MyFile.etl -on 4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a
            (run the sample and click the "Windows 8.1 behavior" button)
            xperf -stop MySession

            After collecting the ETL file, you can decode the trace using xperf, wpa,
            or tracerpt. For example, to decode MyFile.etl with tracerpt:

            tracerpt MyFile.etl
            (generates dumpfile.xml)

            Note that decoding TraceLogging events requires Windows 10. Earlier versions
            of Windows can only reliably decode the simple (manifest-based) events.
            */

            /*
            If a LoggingChannel is created using the 1-parameter constructor, it
            will use Windows 8.1 semantics:

            - The channel's ETW Id is always "4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a".
            - The channel's ETW Name is is always
              "Microsoft-Windows-Diagnostics-LoggingChannel".
            - A LoggingChannelName field containing the Channel Name is automatically
              added to each event.
            - Simple events will be written using manifested-event encoding.
            
            Note that complex events can be written even when using Windows 8.1 mode.
            Complex events always use TraceLogging encoding.

            The 1-parameter constructor is marked as obsolete to ensure that
            developers are aware of the changes in semantics. The Windows 10 semantics
            are useful because they enable use of the ETW Provider Id for event
            filtering.
            */

            /*
            If a LoggingChannel is used as a local or member variable, it should
            be closed (disposed) when no longer needed. C# will do this automatically
            if you enclose the usage in a using block.
            */

            // The 1-parameter constructor creates a channel with Windows 8.1 semantics.
#pragma warning disable 618 // Disable warning for use of obsolete LoggingChannel constructor.
            using (var channel = new LoggingChannel("SampleProvider"))
            {
                // The Id for a Windows 8.1-mode channel is always the same.
                // channel.Id == 4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a

                this.DemonstrateLogging(channel);
            }
#pragma warning restore 618
        }

        /// <summary>
        /// Construct a LoggingChannel with the Windows 10 constructor, then
        /// use the LoggingChannel for the scenario. Also show the use cases
        /// for the two Windows 10 constructors.
        /// </summary>
        public void LogWithWin10Constructor()
        {
            /*
            You can collect the events generated by this method with xperf or another
            ETL controller tool. To collect these events in an ETL file:

            xperf -start MySession -f MyFile.etl -on eff1e128-4903-5093-096a-bdc29b38456f
            (run the sample and click the "Windows 10 behavior" button)
            xperf -stop MySession

            After collecting the ETL file, you can decode the trace using xperf, wpa,
            or tracerpt. For example, to decode MyFile.etl with tracerpt:

            tracerpt MyFile.etl
            (generates dumpfile.xml)

            Note that decoding TraceLogging events requires Windows 10. Earlier versions
            of Windows can only reliably decode the simple (manifest-based) events.
            */

            /*
            If a LoggingChannel is created using the 2-parameter or 3-parameter
            constructors, it will use Windows 10 semantics:

            - The channel's ETW Provider Id can be controlled by the developer.
            - The channel's ETW Name is the same as the Channel Name.
            - The LoggingChannelName field is not automatically added to each event.
            - All events will be written using TraceLogging encoding.
            
            The 1-parameter constructor is marked as obsolete to ensure that
            developers are aware of the changes in semantics. The Windows 10 semantics
            are useful because they enable use of the ETW Provider Id for event
            filtering.

            The 2-parameter constructor accepts a channel name and channel options.
            If the options parameter is null, default options are used. At present,
            the only available option is the channel's group GUID. When using the
            2-parameter constructor, the channel's ETW Id is determined by hashing
            the channel name. (The hashing algorithm is the same as the one used by
            the .NET EventSource class.)

            The 3-parameter constructor accepts channel name, channel options, and
            the ETW Id. This constructor should be used when you need your channel
            to use a specific ETW Id and cannot use the default hashed channel name
            as your ETW Id.
            */

            /*
            If a LoggingChannel is used as a local or member variable, it should
            be closed (disposed) when no longer needed. C# will do this automatically
            if you enclose the usage in a using block.
            */

            // The 2-parameter constructor creates a channel with Windows 10 semantics.
            using (var channel = new LoggingChannel(
                "SampleProvider",
                null)) // null means use default options.
            {
                // The Id is generated by hashing the string "SampleProvider".
                // channel.Id == eff1e128-4903-5093-096a-bdc29b38456f

                this.DemonstrateLogging(channel);
            }

            /*
            Demonstrate other (less-common) constructor scenarios:
            */

            // This creates a channel with Windows 10 semantics and declared
            // membership in a provider group.
            using (var channel = new LoggingChannel(
                "SampleProvider",
                new LoggingChannelOptions(new Guid("2e0582f3-d1b6-516a-9de3-9fd79ef952f8")))) // Join a provider group
            {
                // The Id is generated by hashing the string "SampleProvider".
                // channel.Id == eff1e128-4903-5093-096a-bdc29b38456f
            }

            // This creates a channel with Windows 10 semantics and a specific
            // provider Id.
            using (var channel = new LoggingChannel(
                "SampleProvider",
                null,
                new Guid("2e0582f3-d1b6-516a-9de3-9fd79ef952f8")))
            {
                // channel.Id == 2e0582f3-d1b6-516a-9de3-9fd79ef952f8
            }
        }

        /// <summary>
        /// This method demonstrates the LoggingChannel and LoggingActivity APIs.
        /// </summary>
        /// <param name="channel">
        /// The channel to use for the demonstration. This channel may have been
        /// constructed using a Windows 8.1 constructor or a Windows 10 constructor.
        /// The same APIs are supported in both cases, but the ETL events will be
        /// formatted a bit differently depending on how the channel was constructed.
        /// </param>
        private void DemonstrateLogging(LoggingChannel channel)
        {
            // Whenever any ETW session changes the way it is listening to this
            // channel, the LoggingEnable event is fired. For example, this might
            // be called when a session begins listening, changes the level at
            // which it is listening, or stops listening.
            channel.LoggingEnabled += this.OnLoggingEnabled;

            // Log simple string events
            channel.LogMessage("Simple message"); // default level is Verbose
            channel.LogMessage("Simple error", LoggingLevel.Error);

            // Log simple string + integer events.
            channel.LogValuePair("Simple message", 123); // default level is Verbose
            channel.LogValuePair("Simple error", 456, LoggingLevel.Error);

            // The channel.Name property returns the name that was used when the
            // channel was constructed. When running in Windows 10 mode, the name
            // is already set as the provider name, so no LoggingChannelName is
            // automatically added to the event.
            channel.LogMessage(channel.Name);

            // The channel.Id property is new to Windows 10.
            channel.LogMessage(channel.Id.ToString());

            // If you want to avoid the overhead of collecting data when nobody is
            // listening to your channel, check the Enabled property before logging.
            if (channel.Enabled)
            {
                channel.LogMessage(this.CollectExpensiveData());
            }

            // The IsEnabled() method is exactly the same as the Enabled property,
            // except that it is a new Windows 10 API.
            if (channel.IsEnabled())
            {
                channel.LogMessage(this.CollectExpensiveData());
            }

            // If you want to only collect data if somebody is listening at a specific
            // level, you need to check both Enabled and Level. Note that the value of
            // the Level is unspecified when Enabled is false.
            if (channel.Enabled && channel.Level <= LoggingLevel.Warning)
            {
                channel.LogMessage(this.CollectExpensiveData(), LoggingLevel.Warning);
            }

            // The IsEnabled(LoggingLevel) method is a bit nicer than checking both
            // Enabled and Level, but it is only available on Windows 10 or later.
            if (channel.IsEnabled(LoggingLevel.Warning))
            {
                channel.LogMessage(this.CollectExpensiveData(), LoggingLevel.Warning);
            }

            // You can also use IsEnabled to check for keywords.
            if (channel.IsEnabled(LoggingLevel.Information, 0x10))
            {
                channel.LogMessage(this.CollectExpensiveData(), LoggingLevel.Information);
            }

            // Use LoggingFields with the LogEvent method to write complex events.
            var fields = new LoggingFields();
            fields.AddDouble("pi", 3.14159);
            channel.LogEvent(
                "ComplexEvent",
                fields,
                LoggingLevel.Verbose,
                new LoggingOptions(0x10)); // Keywords = 0x10

            // You can add any number of name-value pairs to a fields object, though
            // you may encounter ETW limitations if you add too many. For example,
            // ETW is limited to a maximum event size of 64KB, and the current
            // TraceLogging decoder can handle no more than 128 fields.

            // Performance optimization: You can reuse a LoggingFields object to
            // avoid unnecessary allocations. Don't forget to call Clear()
            // between uses, and don't try to share a LoggingFields object between
            // threads.
            fields.Clear();
            fields.AddDateTime("Date", DateTimeOffset.Now);
            channel.LogEvent("Now", fields);

            fields.Clear();

            // You can add a formatting hint to affect the way a value is decoded.
            // Not all combinations are useful, and the hint may be ignored.
            // For example, you can encode an MBCS string by writing a byte array
            // with a String hint.
            fields.AddUInt8Array(
                "AnsiString",
                new byte[] { 65, 66, 67, 49, 50, 51 }, // "ABC123"
                LoggingFieldFormat.String);

            // You can add "tag" bits to a field. These are user-defined bits that
            // can be used to communicate with an event processing tool. For example,
            // you might define a tag bit to indicate that a field contains private
            // data that should not be displayed on-screen.
            fields.AddString("Password", "12345", LoggingFieldFormat.Default, 0x10);

            // You can add a "structure" to an event. A structure is a name for a
            // group of fields. Structures can nest. Call BeginStruct to add a level
            // of nesting, and call EndStruct after the last field of the structure.
            fields.BeginStruct("Nested");
                fields.AddInt16("Nested-1", 1);
                fields.AddInt16("Nested-2", 2);
                fields.BeginStruct("Nested-Nested");
                    fields.AddInt16("Nested-Nested-3", 3);
                fields.EndStruct();
                fields.AddInt16("Nested-4", 4);
            fields.EndStruct();

            // Advanced scenarios: you can use a LoggingOptions object to control
            // detailed event settings such as keywords, opcodes, and activity Ids.
            // These have their normal ETW semantics. You can also set event tags,
            // which are bit values that can be used to communicate with the event
            // processor.
            channel.LogEvent(
                "VeryComplexEvent",
                fields,
                LoggingLevel.Information,
                new LoggingOptions { Keywords = 0x123, Tags = 0x10 });

            // Windows 10 introduces the ILoggingTarget interface. LoggingChannel
            // implements this interface. This interface allows components to accept
            // a logger as an parameter.
            this.DoSomething(channel);

            /*
            If a LoggingActivity is created using a LoggingActivity constructor,
            it will use Windows 8.1 semantics:

            - If an activity is destroyed (garbage-collected) without being closed
              and the associated LoggingChannel is still open, the activity will
              write a default Stop event.
            - The default Stop event (written by the destructor or by the Close()
              method) is encoded as a "simple" event.

            The 8.1 semantics are deprecated because the automatic generation of
            a Stop event at garbage-collection can be misleading. The Stop event
            is intended to mark the a precise point at which an activity is
            completed, while the garbage-collection of an abandoned activity is
            inherently imprecise and unpredictable.

            If a LoggingActivity is created using a StartActivity method, it will
            use Windows 10 semantics:

            - If an activity is destroyed (garbage-collected) without being closed,
              there will be no Stop event for the activity.
            - The default Stop event (written by the Close() method) is encoded as
              a TraceLogging event with name "ActivityClosed".
            */

            // This activity is created with Windows 8.1 semantics.
            using (var a1 = new LoggingActivity("Activity1", channel))
            {
                // The activity Start event is written by the LoggingActivity constructor.
                // You would do your activity's work here.
                // The activity Stop event is written when the activity is closed (disposed).

                // The Windows 10 LoggingActivity adds new methods for writing events
                // that are marked as associated with the activity.
                a1.LogEvent("Activity event");

                // LoggingActivity also implements the ILoggingTarget interface, so you can
                // use either a channel or an activity as a logging target.
                this.DoSomething(a1);

                // The Windows 10 LoggingActivity adds new methods for creating nested activities.
                // Note that nested activities are always created with Windows 10 semantics,
                // even when nested under an activity that is using Windows 8.1 semantics.
                using (var a2 = a1.StartActivity("Activity2"))
                {
                    // Nested task occurs here.

                    // The Windows 10 LoggingActivity allows you to customize the Stop event.
                    a2.StopActivity("Activity 2 stop");
                }

                // Because a1 is using Windows 8.1 semantics, if we did not call Dispose(),
                // it would attempt to write a Stop event when it is garbage collected.
                // Writing Stop events during garbage collection is not useful, so be sure
                // to properly stop, close, or dispose activities.
            }

            // The Windows 10 StartActivity method creates a new activity, optionally with
            // specified fields and characteristics.
            // This activity is created with Windows 10 semantics.
            using (var a3 = channel.StartActivity("Activity3"))
            {
                // Because a3 is using Windows 10 semantics, if we did not call Dispose(),
                // there would be no Stop event (not even when the activity is garbage
                // collected). To get a Stop event, be sure to stop, close, or dispose the
                // activity.
            }
        }

        private string CollectExpensiveData()
        {
            return "ExpensiveData";
        }

        private void DoSomething(ILoggingTarget logger)
        {
            logger.LogEvent("Did something");
        }

        private void OnLoggingEnabled(ILoggingChannel sender, object args)
        {
            // Here, you could note a change in the level or keywords.
        }
    }
}
