using galaxy_cli.DTO.Crews;
using galaxy_cli.DTO;
using Spectre.Console;

public static class ConsoleUiHelper
{

    public static Grid BuildCrewGrid(CrewSummaryDTO crew)
    {

        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap().PadRight(4))
            .AddColumn()
            .AddRow("[b][grey]Crew Id[/][/]", $"{crew.Crew_Id}")
            .AddRow("[b][grey]Crew Name[/][/]", $"{crew.Name}")
            .AddRow("[b][grey]Availability[/][/]", crew.Is_Available ? "Available" : "Not available")
            .AddEmptyRow();

        var simple = new Table()
        .Title("[[ [yellow]Crew Members[/] ]]")
        .Border(TableBorder.SimpleHeavy)
        .BorderColor(Color.Yellow)
        .AddColumn("Name")
        .AddColumn("Status")
        .AddColumn("Rank");

        crew.Members.ForEach(member => simple.AddRow($"{member.Full_Name}", member.Is_Active ? "Active" : "Not Active", $"{member.Rank?.Title ?? ""}"));

        grid.AddRow(simple);

        return grid;
    }

    public static Grid BuildGalaxyGrid(GalaxyDTO galaxy)
    {
        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap().PadRight(4))
            .AddColumn()
            .AddRow("[b][grey]Galaxy Id[/][/]", $"{galaxy.Galaxy_Id}")
            .AddRow("[b][grey]Name[/][/]", $"{galaxy.Name}")
            .AddRow("[b][grey]Type[/][/]", $"{galaxy.GalaxyTypeName}")
            .AddRow("[b][grey]Distance From Earth[/][/]", $"{galaxy.Distance_From_Earth}")
            .AddRow("[b][grey]Description[/][/]", $"{galaxy.Description}");

        return grid;
    }

}