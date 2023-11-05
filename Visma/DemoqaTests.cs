using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;

namespace Visma
{
    public class TestForm
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public TestForm(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public bool TryClickElement(By ClickedElement)
        {
            try
            {
                IWebElement element = driver.FindElement(ClickedElement);
                element.Click();
                return true;  // Return true if click succeeds
            }
            catch (ElementClickInterceptedException)
            {
                return false;  // Return false if ElementClickInterceptedException is thrown
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;  // Re-throw exception if it's not an ElementClickInterceptedException
            }
        }

        public void FillPersonalInfo(string firstName, string lastName, string email, string gender, string mobile, string address, string hobby)
        {
            driver.FindElement(By.Id("firstName")).SendKeys(firstName);
            driver.FindElement(By.Id("lastName")).SendKeys(lastName);
            driver.FindElement(By.Id("userEmail")).SendKeys(email);
            driver.FindElement(By.XPath($"//label[contains(text(),'{gender}')]")).Click();
            driver.FindElement(By.Id("userNumber")).SendKeys(mobile);
            driver.FindElement(By.Id("currentAddress")).SendKeys(address);
            driver.FindElement(By.XPath($"//label[contains(text(),'{hobby}')]")).Click();
        }

        public void FillState(string state, string city)
        {     
            bool isClickable = TryClickElement(By.Id("state"));
            if (isClickable)
            {
                driver.FindElement(By.Id("state")).Click();
                driver.FindElement(By.Id("react-select-3-input")).SendKeys(state);
                driver.FindElement(By.Id("react-select-3-input")).SendKeys((Keys.Enter));

                driver.FindElement(By.Id("city")).Click();
                driver.FindElement(By.Id("react-select-4-input")).SendKeys(city);
                driver.FindElement(By.Id("react-select-4-input")).SendKeys((Keys.Enter));
            }
            else
            {
                driver.FindElement(By.Id("currentAddress")).SendKeys((Keys.Tab));
                driver.FindElement(By.Id("react-select-3-input")).SendKeys(state);
                driver.FindElement(By.Id("react-select-3-input")).SendKeys((Keys.Enter));

                driver.FindElement(By.Id("currentAddress")).SendKeys((Keys.Tab + Keys.Tab));
                driver.FindElement(By.Id("react-select-4-input")).SendKeys(city);
                driver.FindElement(By.Id("react-select-4-input")).SendKeys((Keys.Enter));
            }
            
         }

        public void FillDateOfBirth(string month, string year, string day,string date)
        {
            driver.FindElement(By.Id("dateOfBirthInput")).Click();

            IWebElement monthDropdown = wait.Until(drv => drv.FindElement(By.ClassName("react-datepicker__month-select")));
            new SelectElement(monthDropdown).SelectByValue(month);

            IWebElement yearDropdown = wait.Until(drv => drv.FindElement(By.ClassName("react-datepicker__year-select")));
            new SelectElement(yearDropdown).SelectByValue(year);

            IWebElement dayElement = wait.Until(drv => drv.FindElement(By.XPath($"//div[contains(@class, 'react-datepicker__day') and not(contains(@class, 'react-datepicker__day--outside-month')) and text()='{day}']")));
            dayElement.Click();
        }

        public void FillDateOfBirthIssue(string date)
        {
            driver.FindElement(By.Id("dateOfBirthInput")).Clear();
            driver.FindElement(By.Id("dateOfBirthInput")).SendKeys(date);
            driver.FindElement(By.Id("dateOfBirthInput")).SendKeys((Keys.Enter));

            String textAfterChangingDate = driver.FindElement(By.Id("dateOfBirthInput")).GetAttribute("value");
            int textAfterChangingDateLenght = textAfterChangingDate.Length;

            if (textAfterChangingDateLenght > 11 || textAfterChangingDate != date)
            {
                throw new Exception("Date of birth input did not change as expected.");
            }
        }

        public void DateOfBirthWithBackspace()
        {
            String text = driver.FindElement(By.Id("dateOfBirthInput")).GetAttribute("value");

            while (!string.IsNullOrEmpty(text))
            {
                driver.FindElement(By.Id("dateOfBirthInput")).SendKeys(Keys.Backspace);
                text = driver.FindElement(By.Id("dateOfBirthInput")).GetAttribute("value");
            }

            driver.FindElement(By.Id("dateOfBirthInput")).Click();
            driver.FindElement(By.Id("dateOfBirthInput")).SendKeys("1.1.2000");

        }

        public void FillHobbies(string subjectName)
        {
            // Find the 'Subjects' Input Field and Type a Subject
            var subjectsInput = driver.FindElement(By.Id("subjectsInput"));
            subjectsInput.Clear();
            subjectsInput.SendKeys(subjectName);
            subjectsInput.SendKeys((Keys.Enter));
        }

        public void TestFileUpload(string imgPath)
        {
            // Find the 'Choose File' button and set the file path
            IWebElement fileInput = driver.FindElement(By.Id("uploadPicture"));
            fileInput.SendKeys(imgPath);
        }
        
        public void SubmitFormular()
        {
            driver.FindElement(By.Id("submit")).SendKeys((Keys.Enter));
        }

        public void VerificationOfFillFormular(string lableElement, string requestedLableValue)
        {
            // First, locate the 'Label' cell that contains 'Student Name'
            IWebElement labelElement = driver.FindElement(By.XPath($"//td[text()='{lableElement}']"));

            // Now, locate the adjacent 'Values' cell which contains the actual student name
            IWebElement valueElement = labelElement.FindElement(By.XPath("following-sibling::td"));

            // Get the text of the 'Values' cell
            string studentName = valueElement.Text;

            // Your expected value
            string expectedName = requestedLableValue;

            // Compare the values
            if (studentName.Equals(expectedName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("The names match!");
            }
            else
            {
                Console.WriteLine($"Expected: {expectedName}, but got: {studentName}");
            }

            driver.FindElement(By.Id("closeLargeModal")).Click();
        }

        public bool wontCreatFormular()
        {
            //verify if pop-up formular was created when user enter wrong email adress
            bool isElementdisplayed = driver.FindElements(By.Id("closeLargeModal")).Count == 0;

            return isElementdisplayed;           
        }
    }


    [TestClass]
    public class BrowserOpenTest
    {
        private IWebDriver driver;
        private TestForm formHelper;

        [TestInitialize]
        public void SetUp()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://demoqa.com/automation-practice-form");
            formHelper = new TestForm(driver);
        }

        [TestMethod]
        public void FillFormTest()
        {
            formHelper.FillPersonalInfo("John", "Doe", "john.doe@example.com", "Male", "1234567890", "Vysokoskolska 253, Kosice 040 01", "Sports");
            formHelper.FillDateOfBirth("1", "2000", "1", "1.1.2000");
            formHelper.FillHobbies("Computer Science");
            formHelper.FillHobbies("Physics");
            formHelper.FillHobbies("English");
            formHelper.TestFileUpload(@"C:\Users\bambu\Pictures\Screenshots\Screenshot 2023-10-29 165728.png");
            formHelper.FillState("NCR", "Delhi");
            formHelper.SubmitFormular();
            formHelper.VerificationOfFillFormular("Student Name", "John Doe");
            
        }

        [TestMethod]
        public void EnterDateOfBirthThroughtInput()
        {
            formHelper.FillPersonalInfo("John", "Doe", "john.doe@example.com", "Male", "1234567890", "Vysokoskolska 253, Kosice 040 01", "Sports");
            formHelper.FillDateOfBirthIssue("01 Jan 2000");
        }

        [TestMethod]
        public void EnterBirthDateWithBackspace()
        {
            formHelper.DateOfBirthWithBackspace();
            formHelper.SubmitFormular();
        }

        [TestMethod]
        public void EnterWrongEmailAdress()
        {
            formHelper.FillPersonalInfo("John", "Doe", "@#$", "Male", "1234567890", "Vysokoskolska 253, Kosice 040 01", "Sports");
            formHelper.SubmitFormular();
            Assert.IsTrue(formHelper.wontCreatFormular());
        }

        [TestMethod]
        public void EnterMissingUserName()
        {
            formHelper.FillPersonalInfo("", "Doe", "john.doe@example.com", "Male", "1234567890", "Vysokoskolska 253, Kosice 040 01", "Sports");
            formHelper.SubmitFormular();
            Assert.IsTrue(formHelper.wontCreatFormular());
        }

        [TestMethod]
        public void NoChooseGender()
        {
            formHelper.FillPersonalInfo("John", "Doe", "john.doe@example.com", "", "1234567890", "Vysokoskolska 253, Kosice 040 01", "Sports");
            formHelper.SubmitFormular();
            Assert.IsTrue(formHelper.wontCreatFormular());
        }

        [TestMethod]
        public void EnterWrongPhoneNumber()
        {
            formHelper.FillPersonalInfo("Jhon", "Doe", "john.doe@example.com", "Male", "0", "Vysokoskolska 253, Kosice 040 01", "Sports");
            formHelper.SubmitFormular();
            Assert.IsTrue(formHelper.wontCreatFormular());
        }

        [TestCleanup]
        public void TearDown()
        {
            Thread.Sleep(1000);
            driver.Quit();
        }
    }
}