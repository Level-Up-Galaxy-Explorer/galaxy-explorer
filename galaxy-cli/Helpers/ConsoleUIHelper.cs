
using galaxy_cli.DTO.Crews;
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

}