namespace SampleCPMProject
{
    using Microsoft.CustomerPreferences.Api.Contracts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class Program
    {
        static IJarvisCPMClient cpmClient;
        static Guid testTopicId = new Guid("00000000-0000-0000-0000-000000000001");

        // don't forget to populate the client secret and client id in App.Config
        public static void Main(string[] args)
        {
            cpmClient = new JarvisCPMClient();

            //Uncomment the method you want to run below.

            try
            {
                //GetEmailContactPoint();
                //GetEmailContactabilities();
                //PatchEmailContactPoint();

                //GetPhoneContactPoint();
                //GetPhoneContactabilities();
                //PatchPhoneContactPoint();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }

            Console.Read();
        }

        /// <summary>
        /// Given a list of email addresses and a particular topic id, check if those email contact values
        /// can be contacted by the topic id.
        /// This end point is pessimistic, in that if CPM does not know about the contact value or if the contact value does not know about the topic,
        /// the "CanContact" value will always return false.
        /// </summary>
        private static void GetEmailContactabilities()
        {
            var emails = new string[] { "test@microsoft.com", "test@pct.com", "abc@test.com" };
            
            EmailContactabilitiesRequest request = new EmailContactabilitiesRequest()
            {
                TargetedTopicId = testTopicId,                                 //Topic Id for which you want to contact customers

                /*If the UnsubscribeUrlRequired field is set to true CPM will return a URL that customers can use to unsubscribe from this communication.
                 * This URL will only be returned for customers for whom canContact = true.
                 * This URL is meant to be included in the email communication that is sent to the customer
                 */
                UnsubscribeUrlRequired = true                                  
            };

            foreach (string emailAddress in emails)
            {
                request.ContactPoints.Add(emailAddress);
            }

            EmailContactabilitiesResponse result = cpmClient.GetEmailContactability(request).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Given an existing email address, looks up the email contact point and prints it out.
        /// </summary>
        private static void GetEmailContactPoint()
        {
            string emailAddress = "test@microsoft.com";
            EmailContactPoint result = cpmClient.GetEmailContactPoint(emailAddress).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Patches an email contact point with a subscription to a topic. Will create a new email contact point if the email contact point does not exist. 
        /// Cannot be used to mark an email contact point as inactive.
        /// </summary>
        private static void PatchEmailContactPoint()
        {
            EmailContactPoint emailContactPoint = new EmailContactPoint()
            {
                Email = "hellow123@fgh.com",
                Country = "US"                                                  //Use only ISO 2 char country codes. Any other string will result in HTTP 400
            };
            emailContactPoint.TopicSettings.Add(new ContactPointTopicSetting
            {
                TopicId = testTopicId,                                          //Topic ID for which this permission was collected
                CultureName = CultureInfo.CurrentCulture.ToString(),            //Specify a culture supported by the topic. E.g en-US, fr-FR, fr-CA etc. Communication with the user will be based on this culture;
                LastSourceSetDate = DateTime.UtcNow,                            //The actual time at which this permission was collected. Could be in the past..
                OriginalSource = "SampleCPMProject",                            //Name of this application that collected the consent. Saved for auditing purposes.
                State = ContactPointTopicSettingState.OptInExplicit             //The permission
            });

            EmailContactPoint updatedContactPoint = cpmClient.PatchEmailContactPoint(emailContactPoint).Result;
            Console.WriteLine(JsonConvert.SerializeObject(updatedContactPoint));
        }

        /// <summary>
        /// Given a phone number, some optional name fields and a particular topic id, check if those contact values
        /// can be contacted by the topic id.
        /// This end point is pessimistic, in that if CPM does not know about the contact value or if the contact value does not know about the topic,
        /// the "CanContact" value will always return false.
        /// </summary>
        private static void GetPhoneContactabilities()
        {
            ContactName name = new ContactName()
            {
                FirstName = "John",
                LastName = "Doe"
            };

            PhoneContactIdentity identity = new PhoneContactIdentity()
            {
                PhoneNumber = "+14256668888",                                   //The phone number should follow the E.164 standard
                Name = name
            };

            PhoneContactabilitiesRequest request = new PhoneContactabilitiesRequest()
            {
                Identity = identity,
                TargetedTopicId = testTopicId
            };

            PhoneContactabilitiesResponse result = cpmClient.GetPhoneContactability(request).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Given an existing phone contact point type and contact point value, looks up the phone contact point and prints it out.
        /// </summary>
        private static void GetPhoneContactPoint()
        {
            ContactName name = new ContactName()
            {
                FirstName = "testfnone",
                MiddleName = "second",
                LastName = "testlnone"
            };

            PhoneContactIdentity identity = new PhoneContactIdentity()
            {
                PhoneNumber = "+1234567890",                                   //The phone number should follow the E.164 standard
                Name = name
            };

            IEnumerable<PhoneContactPoint> result = cpmClient.GetPhoneContactPoint(identity, useFuzzyMatch: false).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Patches a phone contact point with a subscription to a topic. Will create a new phone contact point if the phone contact point does not exist. Cannot be used to mark a phone contact point as inactive.
        /// </summary>
        private static void PatchPhoneContactPoint()
        {
            ContactName name = new ContactName()
            {
                FirstName = "John",
                LastName = "Doe"
            };

            PhoneContactIdentity identity = new PhoneContactIdentity()
            {
                PhoneNumber = "+14256668888",                                   //The phone number should follow the E.164 standard
                Name = name
            };

            PhoneContactPoint phoneContactPoint = new PhoneContactPoint()
            {
                Identity = identity,
                Country = "US"                                                  //Use only ISO 2 char country codes. Any other string will result in HTTP 400
            };

            phoneContactPoint.TopicSettings.Add(new ContactPointTopicSetting
            {
                TopicId = testTopicId,                                          //Topic ID for which this permission was collected
                CultureName = CultureInfo.CurrentCulture.ToString(),            //Specify a culture supported by the topic. E.g en-US, fr-FR, fr-CA etc. Communication with the user will be based on this culture;
                LastSourceSetDate = DateTime.UtcNow,                            //The actual time at which this permission was collected. Could be in the past..
                OriginalSource = "SampleCPMProject",                            //Name of this application that collected the consent. Saved for auditing purposes.
                State = ContactPointTopicSettingState.OptInExplicit             //The permission
            });

            cpmClient.PatchPhoneContactPoint(phoneContactPoint).Wait();
            Console.WriteLine("Phone contact patch successfull");
        }
    }
}