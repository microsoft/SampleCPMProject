namespace SampleCPMProject
{
    using Microsoft.CustomerPreferences.Api.Contracts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class Program
    {
        // comment in and out the methods to test out each functionality
        // don't forget to populate the client secret and client id in App.Config
        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await GetEmailContactabilities(new List<string> { "test@microsoft.com", "test@test.com" }, new Guid("00000000-0000-0000-0000-000000000001"));
                //await GetEmailContactPoint("test@microsoft.com");
                //await PatchEmailContactPoint("test@microsoft.com", "Mars territory", new Guid("00000000-0000-0000-0000-000000000001"), ContactPointTopicSettingState.OptInExplicit);
                //await GetPhoneContactabilities("8611111100000", "John", "Doe", new Guid("00000000-0000-0000-0000-000000000002"));
                //await GetPhoneContactPoint("8611111100000", "John", "Doe", PhoneContactMatchStrategyType.PrioritizedNameElementFuzzyMatch);
                //await PatchPhoneContactPoint("+8611111100000", "John", "Williams", "Doe", "Jr", "China", new Guid("00000000-0000-0000-0000-000000000002"), ContactPointTopicSettingState.OptInExplicit);
                //await GetSmsContactabilities(new List<string> { "8889990000", "1234567890" }, new Guid("00000000-0000-0000-0000-000000000003"));
                //await GetSmsContactPoint("8889990000");
                //await PatchSmsContactPoint("+8889990000", "US", new Guid("00000000-0000-0000-0000-000000000003"), ContactPointTopicSettingState.OptInImplicit);
            }).Wait();
            Console.ReadKey();
        }

        /// <summary>
        /// Given a list of email addresses and a particular topic id, check if those email contact values
        /// can be contacted by the topic id.
        /// This end point is pessimistic, in that if CPM does not know about the contact value or if the contact value does not know about the topic,
        /// the "CanContact" value will always return false.
        /// </summary>
        /// <param name="emailAddresses">The list of email addresses to check. Maximum size 50.</param>
        /// <param name="topicId">The topic id".</param>
        /// <returns></returns>
        private static async Task GetEmailContactabilities(List<string> emailAddresses, Guid topicId)
        {
            EmailContactabilitiesRequest request = new EmailContactabilitiesRequest()
            {
                TargetedTopicId = topicId
            };

            foreach (string emailAddress in emailAddresses)
            {
                request.ContactPoints.Add(emailAddress);
            }

            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/EmailContactabilities", request);

                if (response.IsSuccessStatusCode)
                {
                    EmailContactabilitiesResponse result = await response.Content.ReadAsAsync<EmailContactabilitiesResponse>();
                    Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Given an existing email address, looks up the email contact point and prints it out.
        /// </summary>
        /// <param name="emailAddress">The email address of the email contact point eg. email@address.com</param>
        /// <returns></returns>
        private static async Task GetEmailContactPoint(string emailAddress)
        {
            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpMethod get = new HttpMethod("GET");
                HttpRequestMessage request = new HttpRequestMessage(get, "api/EmailContacts");
                request.Headers.Add("x-ms-filter-email", emailAddress);
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    EmailContactPoint emailContactPoint = await response.Content.ReadAsAsync<EmailContactPoint>();
                    Console.WriteLine(JsonConvert.SerializeObject(emailContactPoint, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Patches an email contact point with a subscription to a topic. Will create a new email contact point if the email contact point does not exist. Cannot be used to mark an email contact point as inactive.
        /// </summary>
        /// <param name="emailAddress">The email address of the email contact point eg. email@address.com</param>
        /// <param name="countryName">Optional, the country of the email contact point</param>
        /// <param name="topicId">the identifier of the topic</param>
        /// <param name="state">The state the subscription should be in eg. OptInExplicit</param>
        /// <returns></returns>
        private static async Task PatchEmailContactPoint(string emailAddress, string countryName, Guid topicId, ContactPointTopicSettingState state)
        {
            EmailContactPoint emailContactPoint = new EmailContactPoint()
            {
                Email = emailAddress,
                Country = countryName
            };
            emailContactPoint.TopicSettings.Add(new ContactPointTopicSetting
            {
                TopicId = topicId,
                CultureName = CultureInfo.CurrentCulture.ToString(),
                LastSourceSetDate = DateTime.UtcNow,
                OriginalSource = "SampleCPMProject",
                State = state
            });
            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpResponseMessage response = await client.PatchAsync("api/EmailContacts", emailContactPoint);
                if (response.IsSuccessStatusCode)
                {
                    EmailContactPoint patched = await response.Content.ReadAsAsync<EmailContactPoint>();
                    Console.WriteLine(JsonConvert.SerializeObject(patched, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Given a phone number, some optional name fields and a particular topic id, check if those contact values
        /// can be contacted by the topic id.
        /// This end point is pessimistic, in that if CPM does not know about the contact value or if the contact value does not know about the topic,
        /// the "CanContact" value will always return false.
        /// </summary>
        /// <param name="phoneNumber">The phone number to check. </param>
        /// <param name="firstName">Optional, First Name. </param>
        /// <param name="lastName">Optional, Last Name".</param>
        /// <param name="topicId">The topic id".</param>
        /// <returns></returns>
        private static async Task GetPhoneContactabilities(string phoneNumber, string firstName, string lastName, Guid topicId)
        {
            ContactName name = new ContactName()
            {
                FirstName = firstName,
                LastName = lastName
            };

            PhoneContactIdentity identity = new PhoneContactIdentity()
            {
                PhoneNumber = phoneNumber,
                Name = name
            };

            PhoneContactabilitiesRequest request = new PhoneContactabilitiesRequest()
            {
                Identity = identity,
                TargetedTopicId = topicId
            };

            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/PhoneContactabilities", request);

                if (response.IsSuccessStatusCode)
                {
                    PhoneContactabilitiesResponse result = await response.Content.ReadAsAsync<PhoneContactabilitiesResponse>();
                    Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Given an existing phone contact point type and contact point value, looks up the phone contact point and prints it out.
        /// </summary>
        /// <param name="phoneNumber">The phone number to check. </param>
        /// <param name="firstName">Optional, First Name. </param>
        /// <param name="lastName">Optional, Last Name".</param>
        /// <param name="matchStrategy">Optional, Exact match or prioritized name fuzzy match strategy".</param>
        /// <returns></returns>
        private static async Task GetPhoneContactPoint(string phoneNumber, string firstName, string lastName, PhoneContactMatchStrategyType matchStrategy)
        {
            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpMethod get = new HttpMethod("GET");
                HttpRequestMessage request = new HttpRequestMessage(get, "api/PhoneContacts");
                request.Headers.Add("x-ms-filter-phone-number", phoneNumber);
                request.Headers.Add("x-ms-filter-first-name", firstName);
                request.Headers.Add("x-ms-filter-last-name", lastName);
                request.Headers.Add("x-ms-filter-matching-algorithm", matchStrategy.ToString());
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    List<PhoneContactPoint> phoneContactPoints = await response.Content.ReadAsAsync<List<PhoneContactPoint>>();
                    Console.WriteLine(JsonConvert.SerializeObject(phoneContactPoints, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Patches a phone contact point with a subscription to a topic. Will create a new phone contact point if the phone contact point does not exist. Cannot be used to mark a phone contact point as inactive.
        /// </summary>
        /// <param name="phoneNumber">The phone number of the phone contact point eg. +1234567890</param>
        /// <param name="firstName">Optional, First Name. </param>
        /// <param name="middleName">Optional, Middle Name".</param>
        /// <param name="lastName">Optional, Last Name".</param>
        /// <param name="generationalSuffix">Optional, suffix eg. Sr".</param>
        /// <param name="countryName">the country of the phone contact point</param>
        /// <param name="topicId">the identifier of the topic</param>
        /// <param name="state">The state the subscription should be in eg. OptInExplicit</param>
        /// <returns></returns>
        private static async Task PatchPhoneContactPoint(string phoneNumber, string firstName, string middleName, string lastName, string generationalSuffix, string countryName, Guid topicId, ContactPointTopicSettingState state)
        {
            ContactName name = new ContactName()
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                GenerationalSuffix = generationalSuffix
            };

            PhoneContactIdentity identity = new PhoneContactIdentity()
            {
                PhoneNumber = phoneNumber,
                Name = name
            };

            PhoneContactPoint phoneContactPoint = new PhoneContactPoint()
            {
                Identity = identity,
                Country = countryName
            };

            phoneContactPoint.TopicSettings.Add(new ContactPointTopicSetting
            {
                TopicId = topicId,
                CultureName = CultureInfo.CurrentCulture.ToString(),
                LastSourceSetDate = DateTime.UtcNow,
                OriginalSource = "SampleCPMProject",
                State = state
            });
            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpResponseMessage response = await client.PatchAsync("api/PhoneContacts", phoneContactPoint);

                if (response.IsSuccessStatusCode)
                {
                    // By design, we do not return the request body after a successful phone contact point patch
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Given a list of phone numbers and a particular topic id, check if those sms contact values
        /// can be contacted by the topic id.
        /// This end point is pessimistic, in that if CPM does not know about the contact value or if the contact value does not know about the topic,
        /// the "CanContact" value will always return false.
        /// </summary>
        /// <param name="phoneNumbers">The list of phone numbers to check. Maximum size 50.</param>
        /// <param name="topicId">The topic id".</param>
        /// <returns></returns>
        private static async Task GetSmsContactabilities(List<string> PhoneNumbers, Guid topicId)
        {
            SmsContactabilitiesRequest request = new SmsContactabilitiesRequest()
            {
                TargetedTopicId = topicId
            };

            foreach (string phoneNumber in PhoneNumbers)
            {
                request.ContactPoints.Add(phoneNumber);
            }

            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/SmsContactabilities", request);

                if (response.IsSuccessStatusCode)
                {
                    SmsContactabilitiesResponse result = await response.Content.ReadAsAsync<SmsContactabilitiesResponse>();
                    Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Given an existing phone number, looks up the sms contact point and prints it out.
        /// </summary>
        /// <param name="phoneNumber">The phone number of the sms contact point eg. +1234567890</param>
        /// <returns></returns>
        private static async Task GetSmsContactPoint(string phoneNumber)
        {
            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpMethod get = new HttpMethod("GET");
                HttpRequestMessage request = new HttpRequestMessage(get, "api/SmsContacts");
                request.Headers.Add("x-ms-filter-phone-number", phoneNumber);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    SmsContactPoint smsContactPoint = await response.Content.ReadAsAsync<SmsContactPoint>();
                    Console.WriteLine(JsonConvert.SerializeObject(smsContactPoint, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Patches a sms contact point with a subscription to a topic. Will create a new sms contact point if the sms contact point does not exist. Cannot be used to mark a sms contact point as inactive.
        /// </summary>
        /// <param name="phoneNumber">The phone number of the sms contact point eg. +1234567890</param>
        /// <param name="countryName">the country of the sms contact point</param>
        /// <param name="topicId">the identifier of the topic</param>
        /// <param name="state">The state the subscription should be in eg. OptInExplicit</param>
        /// <returns></returns>
        private static async Task PatchSmsContactPoint(string phoneNumber, string countryName, Guid topicId, ContactPointTopicSettingState state)
        {
            SmsContactPoint smsContactPoint = new SmsContactPoint()
            {
                Phone = phoneNumber,
                Country = countryName
            };
            smsContactPoint.TopicSettings.Add(new ContactPointTopicSetting
            {
                TopicId = topicId,
                CultureName = CultureInfo.CurrentCulture.ToString(),
                LastSourceSetDate = DateTime.UtcNow,
                OriginalSource = "SampleCPMProject",
                State = state
            });
            using (HttpClient client = await CPMClientGenerator.CreateHttpClientAsync())
            {
                HttpResponseMessage response = await client.PatchAsync("api/SmsContacts", smsContactPoint);
                if (response.IsSuccessStatusCode)
                {
                    SmsContactPoint patched = await response.Content.ReadAsAsync<SmsContactPoint>();
                    Console.WriteLine(JsonConvert.SerializeObject(patched, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }
    }
}