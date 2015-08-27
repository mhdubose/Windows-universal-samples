//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;

using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml.Controls;

namespace PedometerCS
{
    /// <summary>
    /// 'Current step count' illustration page
    /// </summary>
    public sealed partial class Scenario3_CurrentStepCount : Page
    {
        MainPage rootPage = MainPage.Current;

        // Common class ID for pedometers
        Guid PedometerClassId = new Guid("B19F89AF-E3EB-444B-8DEA-202575A71599");

        public Scenario3_CurrentStepCount()
        {
            this.InitializeComponent();
            rootPage.NotifyUser("Hit the 'Get steps count' Button", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Invoked when 'GetCurrentButton' is clicked.
        /// 'ReadingChanged' will not be fired when there is no activity on the pedometer 
        /// and hence can't be reliably used to get the current step count. This handler makes
        /// use of pedometer history on the system to get the current step count of the parameter
        /// </summary>
        async private void GetCurrentButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Determine if we can access pedometers
            var deviceAccessInfo = DeviceAccessInformation.CreateFromDeviceClassId(PedometerClassId);
            if (deviceAccessInfo.CurrentStatus == DeviceAccessStatus.Allowed)
            {
                // Determine if a pedometer is present
                // This can also be done using Windows::Devices::Enumeration::DeviceInformation::FindAllAsync
                var sensor = await Pedometer.GetDefaultAsync();
                if (sensor != null)
                {
                    DateTime dt = DateTime.FromFileTimeUtc(0);
                    int totalStepCount = 0;
                    int lastTotalCount = 0;

                    rootPage.NotifyUser("Retrieving history to get current step counts", NotifyType.StatusMessage);

                    // Disable the button while we get the history
                    GetCurrentButton.IsEnabled = false;

                    DateTimeOffset fromBeginning = new DateTimeOffset(dt);

                    try
                    {
                        var historyReadings = await Pedometer.GetSystemHistoryAsync(fromBeginning);

                        // History always returns chronological list of step counts for all PedometerStepKinds
                        // And each record represents cumulative step counts for that step kind.
                        // So we will use the last set of records - that gives us the cumulative step count for 
                        // each kind and ignore rest of the records
                        PedometerStepKind stepKind = PedometerStepKind.Unknown;
                        DateTimeOffset lastReadingTimestamp;
                        bool resetTotal = false;
                        foreach (PedometerReading reading in historyReadings)
                        {
                            if (stepKind == PedometerStepKind.Running)
                            {
                                // reset the total after reading the 'PedometerStepKind.Running' count
                                resetTotal = true;
                            }

                            totalStepCount += reading.CumulativeSteps;
                            if (resetTotal)
                            {
                                lastReadingTimestamp = reading.Timestamp;
                                lastTotalCount = totalStepCount;
                                stepKind = PedometerStepKind.Unknown;
                                totalStepCount = 0;
                                resetTotal = false;
                            }
                            else
                            {
                                stepKind++;
                            }
                        }

                        ScenarioOutput_TotalStepCount.Text = lastTotalCount.ToString();

                        DateTimeFormatter timestampFormatter = new DateTimeFormatter("shortdate longtime");
                        ScenarioOutput_Timestamp.Text = timestampFormatter.Format(lastReadingTimestamp);

                        rootPage.NotifyUser("Hit the 'Get steps count' Button", NotifyType.StatusMessage);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        rootPage.NotifyUser("User has denied access to activity history", NotifyType.ErrorMessage);
                    }

                    // Re-enable button
                    GetCurrentButton.IsEnabled = true;
                }
                else
                {
                    rootPage.NotifyUser("No pedometers found", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Access to pedometers is denied", NotifyType.ErrorMessage);
            }
        }
    }
}
