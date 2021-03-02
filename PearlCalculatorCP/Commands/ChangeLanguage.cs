using System;
using PearlCalculatorCP.Localizer;
using PearlCalculatorCP.Models;

namespace PearlCalculatorCP.Commands
{
    public class ChangeLanguage : ICommand
    {
        public void OnLinkOutput(Action<ConsoleOutputItemModel> messageSender)
        {
            if (Translator.Instance.CurrentLanguage == Translator.Fallbacklanguage)
            {
                messageSender(new ConsoleOutputItemModel("Msg/i18n", "haven't default language setting file or can't load i18n file"));
                messageSender(new ConsoleOutputItemModel("Msg/i18n", $"{Translator.Fallbacklanguage} currently in use"));
            }
            else
                messageSender(new ConsoleOutputItemModel("Msg/i18n", $"i18n file \"{Translator.Instance.CurrentLanguage}\" loaded"));
        }
        
        public void Excute(string[]? parameters, string? cmdName, Action<ConsoleOutputItemModel> messageSender)
        {
            var len = parameters?.Length ?? 0;
#nullable disable
            var opt = len == 1 ? parameters[0].ToLower() : string.Empty;

            if (parameters is null || len != 1)
            {
                messageSender(DefineCmdOutput.ErrorTemplate($"\"{cmdName}\" don't accept {len} parameters"));
                messageSender(DefineCmdOutput.ErrorTemplate($"optional paras: {Translator.Instance.GetLanguagesOptional()}"));
            }
            else if (Translator.Instance.CurrentLanguage == opt)
                messageSender(DefineCmdOutput.MsgTemplate("you don't need change language"));
            else
            {
                if (opt == "cn" || opt == "tw" || Translator.Instance.Languages.Contains(opt))
                {
                    var lang = opt switch
                    {
                        "cn" => "zh_cn",
                        "tw" => "zh_tw",
                        _ => opt
                    };
                    
                    if (Translator.Instance.LoadLanguage(lang, s => messageSender(DefineCmdOutput.ErrorTemplate(s))))
                        messageSender(DefineCmdOutput.MsgTemplate("change language success"));
                }
                else
                {
                    messageSender(DefineCmdOutput.ErrorTemplate($"language option \"{opt}\" not found"));
                    messageSender(DefineCmdOutput.ErrorTemplate($"optional paras: {Translator.Instance.GetLanguagesOptional()}"));
                }
            }
        }
    }
}