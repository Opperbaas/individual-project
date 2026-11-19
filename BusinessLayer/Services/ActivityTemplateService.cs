namespace BusinessLayer.Services;

public sealed class ActivityTemplateService : IActivityTemplateService
{
    public bool IsCompatible(string executionType, string locationType)
    {
        return (executionType, locationType) switch
        {
            ("Teamsporten", "Voetbalveld") => true,
            ("Teamsporten", "Sporthal") => true,
            ("Teamsporten", "Stadion") => true,
            ("Teamsporten", "Openluchtgebied") => true,
            ("Teamsporten", "Basketbalhal") => true,
            ("Teamsporten", "Tennisbaan") => true,
            ("Individuele sporten", "Tennisbaan") => true,
            ("Individuele sporten", "Zwembad") => true,
            ("Individuele sporten", "Atletiekbaan") => true,
            ("Individuele sporten", "Fitnessruimte") => true,
            ("Individuele sporten", "Sporthal") => true,
            ("Individuele sporten", "Openluchtgebied") => true,
            ("Fit- en gymsporten", "Fitnessruimte") => true,
            ("Fit- en gymsporten", "Sporthal") => true,
            ("Fit- en gymsporten", "Openluchtgebied") => true,
            ("Fit- en gymsporten", "Stadion") => true,
            ("Avontuurlijke en risicosporten", "Openluchtgebied") => true,
            ("Avontuurlijke en risicosporten", "Stadion") => true,
            ("Lucht- en watersporten", "Zwembad") => true,
            ("Lucht- en watersporten", "Openluchtgebied") => true,
            ("Lucht- en watersporten", "Stadion") => true,
            ("Balsporten en racket-/batjesporten", "Voetbalveld") => true,
            ("Balsporten en racket-/batjesporten", "Sporthal") => true,
            ("Balsporten en racket-/batjesporten", "Tennisbaan") => true,
            ("Balsporten en racket-/batjesporten", "Basketbalhal") => true,
            ("Strand- en recreatiesporten", "Openluchtgebied") => true,
            ("Strand- en recreatiesporten", "Voetbalveld") => true,
            ("Strand- en recreatiesporten", "Sporthal") => true,
            _ => false
        };
    }

    public string GetActivityName(string executionType, string locationType)
    {
        return (executionType, locationType) switch
        {
            ("Teamsporten", "Voetbalveld") => "Voetbalwedstrijd",
            ("Teamsporten", "Sporthal") => "Teamzaaltoernooi",
            ("Teamsporten", "Stadion") => "Grote teamcompetitie",
            ("Teamsporten", "Openluchtgebied") => "Buitenteamwedstrijd",
            ("Teamsporten", "Tennisbaan") => "Dubbelseizoen",
            ("Teamsporten", "Basketbalhal") => "Basketbaltoernooi",
            ("Individuele sporten", "Tennisbaan") => "Tennistoernooi",
            ("Individuele sporten", "Zwembad") => "Zwemmomento",
            ("Individuele sporten", "Atletiekbaan") => "Atletiekwedstrijd",
            ("Individuele sporten", "Fitnessruimte") => "Individuele trainingssessie",
            ("Individuele sporten", "Sporthal") => "Individuele zaalwedstrijd",
            ("Individuele sporten", "Openluchtgebied") => "Recreatieve looptocht",
            ("Fit- en gymsporten", "Fitnessruimte") => "Fitnesstraining",
            ("Fit- en gymsporten", "Sporthal") => "Gymnastiekcircuit",
            ("Fit- en gymsporten", "Openluchtgebied") => "Buitenfitness",
            ("Fit- en gymsporten", "Stadion") => "Groepsconditietraining",
            ("Avontuurlijke en risicosporten", "Openluchtgebied") => "Avontuurtocht",
            ("Avontuurlijke en risicosporten", "Stadion") => "Avontuurlijk obstakeltoernooi",
            ("Lucht- en watersporten", "Zwembad") => "Aquaticsessie",
            ("Lucht- en watersporten", "Openluchtgebied") => "Watersportclinic",
            ("Lucht- en watersporten", "Stadion") => "Lucht- en watersportdemonstratie",
            ("Balsporten en racket-/batjesporten", "Voetbalveld") => "Balspeltoernooi",
            ("Balsporten en racket-/batjesporten", "Sporthal") => "Racket- en balsportdag",
            ("Balsporten en racket-/batjesporten", "Tennisbaan") => "Racketclinic",
            ("Balsporten en racket-/batjesporten", "Basketbalhal") => "Balvaardigheidsdag",
            ("Strand- en recreatiesporten", "Openluchtgebied") => "Recreatieve strandactiviteit",
            ("Strand- en recreatiesporten", "Voetbalveld") => "Recreatieve veldspelen",
            ("Strand- en recreatiesporten", "Sporthal") => "Recreatieve zaalspelen",
            _ => "Algemene sportactiviteit"
        };
    }
}
