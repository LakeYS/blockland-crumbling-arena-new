////Crumbling Arena Achievements////

// 1. Achievement List

$CA::AchievementCount = 2;

$CA::AchievementName[0] = "Nerf This!";
$CA::AchievementDesc[0] = "Die from an explosive brick.";

$CA::AchievementName[1] = "Mario";
$CA::AchievementDesc[1] = "Jump on someone's head!";

$CA::AchievementName[2] = "Mario II";
$CA::AchievementDesc[2] = "Jump on two heads in a row!";

// 1. Achievement Functions

function serverCmdAchievements(%client)
{
	for(%i=0;%i<=$CA::AchievementCount;%i++)
		messageClient(%client,'',"\c3" @ $CA::AchievementName[%i] @ "\c5 - " @ $CA::AchievementDesc[%i]);
	
	messageClient(%client,'',"\c3Press Page Up and Page Down to scroll in chat.");

}

function awardAchievement(%client,%id)
{
	%aName = $CA::AchievementName[%id];
	%aDesc = $CA::AchievementDesc[%id];
	
	if(%aName $= "" || %aDesc $= "")
	{
		error("awardAchievement - Missing achievement name and/or description for ID \"" @ %id @ "\"!");
		return;
	}
	
	%list = $CA::ScoreAchievements[%client.bl_id]; // Get the list of the client's achievements
	if(getWord(%list,%id) != 1) // Make sure they don't already have it.
	{
		$CA::ScoreAchievements[%client.bl_id] = setWord($CA::ScoreAchievements[%client.bl_id],%id,1);
		messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3" @ $CA::AchievementName[%id] @ " \c5achievement!");
		%client.player.achievementEarned[%id] = 1;
		
		return 1;
	}
}

// Crumbling Arena: Award achievements for the end of a round.
function awardRoundEndAchievements(%client)
{
	%blid = %client.bl_id;
	// None yet
}