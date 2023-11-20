using Oboteration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oboteration.Funcs
{
    public class ReadLogFile
    {
        public async Task<LogFile> getLogInfo(string file)
        {  

            LogFile logFile =  await Task.Run(() =>
            {
                LogFile logFile = new LogFile();
                using (StringReader sr = new StringReader(file))
                {
                    string line = sr.ReadLine();
                    string lastWarning = "";
                    string lastError = "";
                    string panic = "";

                    char typeWep = ' ';
                    bool isLineWEPI = false;

                    // Silly checks for each line of the file
                    while (true)
                    {
                        // Checking for last line
                        if (line == null)
                        {
                            break;
                        }

                        //Reseting the check
                        isLineWEPI = false;
                        switch (line)
                        {
                            //Check for matches of the LogFile variables
                            case string x when x.StartsWith("Application Title") && !x.StartsWith("Application TitleID"):
                                string appTitle = line.Split(':')[1].Trim();
                                if (appTitle == "")
                                {
                                    logFile.AppTitle = null;
                                }
                                else
                                {
                                    logFile.AppTitle = appTitle;
                                }
                                line = sr.ReadLine();
                                break;
                            case string x when x.StartsWith("Application ID") || x.StartsWith("Application TitleID"):
                                string appID = line.Split(':')[1].Trim();
                                if (appID == "")
                                {
                                    logFile.AppID = null;
                                }
                                else
                                {
                                    logFile.AppID = appID;
                                }
                                line = sr.ReadLine();
                                break;
                            case string x when x.StartsWith("Operating System"):
                                string os = line.Split(':')[1].Trim();
                                if (os == "")
                                {
                                    logFile.OS = null;
                                }
                                else
                                {
                                    logFile.OS = os;
                                }
                                line = sr.ReadLine();
                                break;
                            case string x when x.StartsWith("CPU Information"):
                                string cpu = line.Split(':')[1].Trim();
                                if (cpu == "")
                                {
                                    logFile.CpuInfo = null;
                                }
                                else
                                {
                                    logFile.CpuInfo = cpu;
                                }
                                line = sr.ReadLine();
                                break;
                            case string x when x.StartsWith("Memory Available"):
                                string ram = line.Split(':')[1].Trim();
                                if (ram == "")
                                {
                                    logFile.RamInfo = null;
                                }
                                else
                                {
                                    logFile.RamInfo = ram;
                                }
                                line = sr.ReadLine();
                                break;
                            case string x when x.StartsWith("++++++++++++++++++ E"):
                                typeWep = 'e';
                                isLineWEPI = true;
                                lastError = line;
                                break;
                            case string x when x.StartsWith("++++++++++++++++++ W"):
                                typeWep = 'w';
                                isLineWEPI = true;
                                lastWarning = line;
                                break;
                            case string x when x.StartsWith("++++++++++++++++++ P"):
                                typeWep = 'p';
                                isLineWEPI = true;
                                panic = line;
                                break;
                            case string x when x.StartsWith("++++++++++++++++++ I"):
                                typeWep = 'i';
                                isLineWEPI = true;
                                break;
                            default:
                                line = sr.ReadLine();
                                break;
                        }
                        if (isLineWEPI)
                        {
                            while (true)
                            {
                                line = sr.ReadLine();

                                //If line is a WEPI or part of the needed info no need to do checks
                                if (line == null || line.StartsWith("++++++++++++++++++") || line.StartsWith("Application Title") 
                                || line.StartsWith("Application ID")|| line.StartsWith("Application TitleID") 
                                || line.StartsWith("Operating System")|| line.StartsWith("CPU Information") 
                                || line.StartsWith("Memory Available"))
                                {
                                    break;
                                }

                                //Check where to add the line
                                switch (typeWep)
                                {
                                    case 'e':
                                        lastError = lastError + "\n" + line;
                                        break;
                                    case 'w':
                                        lastWarning = lastWarning + "\n" + line;
                                        break;
                                    case 'p':
                                        panic = panic + "\n" + line;
                                        break;
                                    case 'i':
                                        break;
                                }
                            }
                        }

                        // Assigning the wep according to priority
                        if (panic != "")
                        {
                            logFile.Wep = panic;
                        }
                        else if (lastError != "" && line == null)
                        {
                           logFile.Wep = lastError;
                        }
                        else if (lastWarning != "" && line == null)
                        {
                            logFile.Wep = lastWarning;
                        }
                    }
                }
                return logFile;
            });
            
            return logFile;
        }
        public async Task<bool> checkValid(string file)
        {
            bool isVlid = await Task.Run(() =>
            {
                using (StringReader sr = new StringReader(file))
                {
                    string line;
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null)
                        {
                            return false;
                        }
                        if (line.StartsWith("++++++++++++++++++"))
                        {
                            return true;
                        }
                    }
                }
            });
            return isVlid;
        }
    }
    
}
