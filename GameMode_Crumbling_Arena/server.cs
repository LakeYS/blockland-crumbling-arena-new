exec("./arenaGenerator.cs");
exec("./achievements.cs");

playerNoJet.minImpactSpeed = 4;
playerNoJet.groundImpactShakeAmp = "0.2 0.2 0.2";

//if(getSubStr(getDateTime(),0,2) == 12)
//{
//	$CA::XmasStuff = 1;
//	exec("Add-Ons/Brick_Christmas_Tree/server.cs");
//}

exec("./Support_CustomRangeMusic.cs");
createMusicDatablocks();

datablock fxDTSBrickData(brick2xCubeData)
{
	brickFile = "./2x Cube.blb";
	category = "Baseplates";
	subCategory = "Cube";
	uiName = "2x Cube";
	iconName = "";
};

datablock AudioDescription(Audio15xMusicLooping3d : AudioMusicLooping3d)
{
	maxDistance = 450;
	referenceDistance = 50;
};

datablock AudioProfile(brickStep0)
{
	filename = "./brickStep0.wav";
	description = AudioClose3d;
	preload = true;
};

datablock fxDTSBrickData(brick15xMusicData : brickMusicData)
{
	uiName = "Music (15x Range)";
	musicRange = 450;
	musicDescription = Audio15xMusicLooping3d;
};

if(!$CA::HasLoaded)
{
	$CA::HasLoaded = 1;

	if(isFunction(RTB_registerPref))
		RTB_registerPref("Enable Music", "Crumbling Arena", "$CA::Music", "bool", "GameMode_Crumbling_Arena", "1", 0, 0);
	else
		$CA::Music = 1;

	%filename = "config/server/CrumbleArena/scores.cs";
	if(isFile(%filename))
		exec(%filename);

	new PhysicalZone(CAGravityZone)
	{
		position = "-50000 50000 -100";
		rotation = "1 0 0 0";
		scale = "1e+006 1e+006 1000";
		velocityMod = "1";
		gravityMod = "1";
		extraDrag = "0";
		isWater = "0";
		waterViscosity = "40";
		waterDensity = "1";
		waterColor = "0 0 0 0";
		appliedForce = "0 0 0";
		polyhedron = "0 0 0 1 0 0 0 -1 0 0 0 1";
	};
	missiongroup.add(CAGravityZone);
	CAGravityZone.activate();

	if(isFile("Add-Ons/Server_HatMod/server.cs"))
	{
		$CA::Hatmod = 1;
		exec("Add-Ons/Server_HatMod/server.cs");
	}

	schedule(1000,0,createMusicDB);
	schedule(1000,0,deleteGround);
}

function deleteGround()
{
	if(isObject(groundPlane))
		groundPlane.delete();
}

function createMusicDB()
{
	if(isObject(MusicData))
	{
		for(%i=0;%i<MusicData.getCount();%i++)
			MusicData.getObject(%i).delete();

		MusicData.delete();
	}

	new SimGroup(MusicData) { name = "MusicData"; };

	for(%i=0;%i<DatablockGroup.getCount();%i++)
	{
		%obj = DatablockGroup.getObject(%i);
		if(strStr(%obj.getName(),"musicData") != -1)
		{
			%loop = %obj.getName();

			%data = new ScriptObject(MusicNameData)
			{

				music = %loop;
				corrected = %loop.uiName;

			};
			MusicData.add(%data);
		}
	}
}

function makeNewSpawn(%x,%y,%z,%b1,%b2)
{
	%firstPos = %b1.getPosition();
	%lastPos = %b2.getPosition();

	for(%i=0;%i<clientGroup.getCount();%i++)
	{
		%client = ClientGroup.getObject(%i);

		// To do: Fix round starting (The function can't use $CA::ClientCount to check)
		//if(%client.awaitingSpawnPrompt)
		//{
		//	%client.player.delete();
		//	echo("PLACEHOLDER: awaiting prompt");
		//}
		//else if($DefaultMinigame.isMember(%client)) // Make sure they're in the minigame and not loading. (Just in case, we'll also make sure they're actually in the right minigame)
		if($DefaultMinigame.isMember(%client)) // Make sure they're in the minigame and not loading. (Just in case, we'll also make sure they're actually in the right minigame)
		{
			//%client.player.delete();
			%client.spawnPlayer();
			%client.player.addVelocity("0 0 -1");

			%client.player.tool[0] = "";
			messageClient(%client,'MsgItemPickup','',0,0);

			%client.player.spawnPoint = getRandom(getWord(%firstPos,0)+15,getWord(%lastPos,0)-15) SPC getRandom(getWord(%firstPos,1)+15,getWord(%lastPos,1)-15) SPC %z;
			%client.player.setTransform(%client.player.spawnPoint);
		}
		else
			$CA::ClientCount--; // Not in the minigame for some reason, don't count them.
	}

	cancel($CA::Loop::Modifier);
	$CA::Loop::Modifier = schedule($CA::GameDelay,0,doRoundModifier,getRandom(1,14)); //getRandom(1,14)
	//$CA::Loop::Modifier = schedule($CA::GameDelay,0,doRoundModifier,11);

	$CA::BrickCount = getBrickCount();

	$CA::Start = getSimTime();

	if($CA::ClientCount == 1)
		$CA::SoloRoundStarted = 1;
}

function doRoundModifier(%which)
{
	//talk("doRoundModifier" SPC %which);
	centerPrintAll("<font:impact:60>\c3BEGIN!",5);
	$CA::RoundModifierID = %which;

	switch(%which)
	{
		case 1: //Pushbrooms
			if($CA::ClientCount > 1)
			{
				for(%i=0;%i<clientGroup.getCount();%i++)
				{
					%client = clientGroup.getObject(%i);

					if(isObject(%client.player))
					{
						%client.player.tool[0] = PushBroomItem.getID();
						messageClient(%client,'MsgItemPickup','',0,PushBroomItem.getID());
					}
				}
				centerPrintAll("<font:impact:60>\c3Pushbrooms!",5);
			}

		case 2: //Huge
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%client = clientGroup.getObject(%i);

				if(isObject(%client.player))
					%client.player.setPlayerScale("1.75 1.75 1.75");
			}
			centerPrintAll("<font:impact:60>\c3Giants!",5);

		case 3: //Auto crumble
			centerPrintAll("<font:impact:60>\c3The arena is unstable!",5);
			$CA::Crumble = 1;
			$CA::FTWarn = 1;

		case 4: //Small playerscale
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%client = clientGroup.getObject(%i);

				if(isObject(%client.player))
					%client.player.setPlayerScale("0.6 0.6 0.6");
			}
			centerPrintAll("<font:impact:60>\c3Haha, alright then shortstuffs.",5);

		case 5: //Horses
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%client = clientGroup.getObject(%i);

				if(isObject(%client.player))
					%client.player.setDatablock(HorseArmor);
			}
			centerPrintAll("<font:impact:60>\c3Horses, because horses.",5);

		case 6: //Swords
			if($CA::ClientCount > 1)
			{
				for(%i=0;%i<clientGroup.getCount();%i++)
				{
					%client = clientGroup.getObject(%i);

					if(isObject(%client.player))
					{
						%client.player.tool[0] = SwordItem.getID();
						messageClient(%client,'MsgItemPickup','',0,SwordItem.getID());
					}
				}
				centerPrintAll("<font:impact:60>\c3Swords!",5);
			}
		case 7: //Slow
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%client = clientGroup.getObject(%i);

				if(isObject(%client.player))
				{
					%d = %client.player.dataBlock;

					%client.player.setMaxForwardSpeed(%d.maxForwardSpeed/2);
					%client.player.setMaxBackwardSpeed(%d.maxBackwardSpeed/2);
					%client.player.setMaxSideSpeed(%d.maxSideSpeed/2);

					%client.player.setMaxCrouchForwardSpeed(%d.maxForwardCrouchSpeed/2);
					%client.player.setMaxCrouchBackwardSpeed(%d.maxBackwardCrouchSpeed/2);
					%client.player.setMaxCrouchSideSpeed(%d.maxSideCrouchSpeed/2);
				}
			}
			centerPrintAll("<font:impact:60>\c3Slowpokes!",5);
		case 8: //Fast
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%client = clientGroup.getObject(%i);

				if(isObject(%client.player))
				{
					%d = %client.player.dataBlock;

					%client.player.setMaxForwardSpeed(%d.maxForwardSpeed*1.8);
					%client.player.setMaxBackwardSpeed(%d.maxBackwardSpeed*1.8);
					%client.player.setMaxSideSpeed(%d.maxSideSpeed*1.8);

					%client.player.setMaxCrouchForwardSpeed(%d.maxForwardCrouchSpeed*1.8);
					%client.player.setMaxCrouchBackwardSpeed(%d.maxBackwardCrouchSpeed*1.8);
					%client.player.setMaxCrouchSideSpeed(%d.maxSideCrouchSpeed*1.8);
				}
			}
		centerPrintAll("<font:impact:60>\c3Gotta go fast!",5);
		case 9: //Low gravity
			CAGravityZone.gravityMod = 0.6;
			CAGravityZone.sendUpdate();
			centerPrintAll("<font:impact:60>\c3SPACE!",5);
		case 10: //Fast crumbling
			$CA::CrumbleTime = 1; // Reduce the delay to 1ms
			centerPrintAll("<font:impact:60>\c3Quicksand!",5);
		case 11: //Minefield
			$CA::CrumbleTime = 1; // Reduce the delay to 1ms
			centerPrintAll("<font:impact:60>\c3It's a minefield!",5);
			for(%i = 0; %i <= BrickGroup_888888.getCount()-1; %i++)
			{
				%brick = BrickGroup_888888.getObject(%i);
				if(getRandom(1,32) == 1) //
				{
					%brick.setEmitter("AdminWandEmitterB");
					%brick.setColorFX(5);
					%brick.isBombBrick = 1;
				}
			}
	}
}

function serverCmdToggleHud(%client)
{
	if(%client.HUD)
		%client.HUD = 0;
	else
		%client.HUD = 1;
}

function serverCmdToggleDebugHud(%client)
{
	if(%client.debugHUD)
		%client.debugHUD = 0;
	else
		%client.debugHUD = 1;
}

function serverCmdToggleMusic(%client)
{
	if(%client.bl_id == 999999 || %client.isAdmin)
	{

		if($CA::Music)
		{
			$CA::Music = 0;
			messageAll('',"\c6Music has been \c0disabled.");
		}
		else
		{
			$CA::Music = 1;
			messageAll('',"\c6Music has been \c2enabled.");
		}
	}
}

function serverCmdStats(%client) // WIP
{
	%blid = %client.bl_id;
	%wins = $CA::Score[%blid];
	%loss = $CA::ScoreLoss[%blid];
	%bricks = $CA::ScoreDestroyed[%blid];
	%achievements = $CA::ScoreAchievements[%blid];

	if(!%wins)
		%wins = 0;

	if(!%loss)
		%loss = 0;

	if(!%bricks)
		%bricks = 0;

	for(%i = 0; %i <= getWordCount(%achievements)-1; %i++)
		if(getWord(%achievements,%i) == 1)
			%achievementCount++;

	if(!%achievements)
		%achievements = 0;

	messageClient(%client,'CAStats',"\c5You have won \c3" @ %wins @ "\c5 times. You have died \c3" @ %loss @ "\c5 times. You have destroyed a total of \c3" @ %bricks @ "\c5 bricks!");
	messageClient(%client,'CAStats',"\c5You have unlocked \c3" @ %achievementCount @ "/" @ $CA::AchievementCount+1 @ "\c5 achievements so far! Type \c3/achievements\c5 for more info.");
}

function serverCmdHelp(%client)
{
	messageClient(%client,'listServerCmd',"\c3/achievements\c6: View all achievements");
	messageClient(%client,'listServerCmd',"\c3/stats\c6: View your stats");
	serverCmdHatHelp(%client);
	messageClient(%client,'',"Type /help to view this list again. (Press Page Up and Page Down to scroll chat)");
}

function serverCmdCommands(%client)
{
	serverCmdHelp(%client);
}

deactivatePackage("CrumblingArenaPackage");
package CrumblingArenaPackage
{
	// sound: The sound to play when the brick is destroyed.
	// player: The player responsible for destroying the brick. If specified, this brick will be counted in their stats for the round.
	// passthrough: set to 1 if brick is already being fake-killed (such as from an explosion)
	function fxDTSBrick::destroyBrick(%this,%force,%sound,%player,%passthrough)
	{
		if(%force)
		{
			%this.isBombBrick = 0;
			%this.bombTriggered = 0;
		}

		if(getSimTime() - $CA::Start > $CA::GameDelay && !%this.crumbled && !%this.bombTriggered)
		{
			if(%this.isBombBrick && isObject(%this) && !%this.crumbled)
			{
				//%this.crumbled = 1;
				%this.bombTriggered = 1;
				%this.setEmitter("BurnEmitterA");
				%this.setLight("AlarmLightA");
				%this.schedule(3500,spawnExplosion,rocketLauncherProjectile,$CA::BrickDatablock.brickSizeX/1.5);
				%this.schedule(3501,destroyBrick,1); // In case the brick mysteriously doesn't explode
				return;
			}

			if(isObject(%player))
				%player.bricksDestroyed++;

			$CA::BrickCount--;
			%this.crumbled = 1;
			%this.setEmitter();
			%this.setLight();
			%this.setColor(%this.colorID+10); // Make the brick transparent

			if(%sound !$= "")
				%this.playSound(%sound);

			if(!%passthrough)
				%this.schedule($CA::CrumbleTime,fakeKillBrick,"0 0 0",3);

			%this.schedule(2500,disappear,-1);
			%this.schedule(3000,delete);
		}
		return;
	}

	function fxDtsBrick::onFakeDeath(%this)
	{
		if(!%this.crumbled)
		{
			if(%this.isBombBrick)
			{
				%this.isBombBrick = 0;
				%this.bombTriggered = 0;
			}

			%this.destroyBrick(0,0,0,0,1);
		}

		Parent::onFakeDeath(%this);
	}

	function fxDtsBrick::onPlayerTouch(%this,%player)
	{
		if(%this && !%player.client.noCrumble)
		{
			// If they touch a brick...
			%player.achievementExplosion = 0; // ...they haven't died from an explosion, no achievement
			%player.achievementMario2 = 0; // ...they aren't jumping on heads, no achievement
			%this.destroyBrick(0,"brickStep0",%player); // Finally, destroy the brick.
		}
	}

	//function fxDTSBrick::onActivate(%brick,%player,%a,%b)
	//{
	//	Parent::onActivate(%brick,%client,%a,%b);
	//}

	function MinigameSO::reset(%minigame,%b,%c,%d,%e,%f,%g)
	{
		BrickGroup_888888.deleteAll();
		buildArena();

		$CA::ClientCount = clientGroup.getCount();

		$CA::FallingTiles = 0;
		$CA::FTWarn = 0;
		$CA::Start = getSimTime();
		$CA::Winner = 0;
		$CA::GameEnded = 0;
		$CA::Delay = 3000;
		$CA::Crumble = 0;
		$CA::RoundStartMessage = 0;
		$CA::SoloRoundStarted = 0;
		$CA::CrumbleTime = 125;

		CAGravityZone.gravityMod = 1;
		CAGravityZone.sendUpdate();
		//CAGravityZone.deactivate();

		export("$CA::Score*","config/server/CrumbleArena/scores.cs");

		parent::reset(%minigame,%b,%c,%d,%e,%f,%g);

		// This is special handling for if there are no clients.
		if($CA::ClientCount == 0)
		{
			// We're going to cancel the minigame's "reset" timer. This is so we don't keep rebuilding arenas every 5 minutes for an empty server.
			$CA::Paused = 1;
			cancel(%minigame.timeLimitSchedule);
		}
	}

	function MinigameSO::checkLastManStanding(%minigame,%b)
	{
		// More special handling for empty servers...
		if($CA::Paused) // If the schedule isn't pending, it's because we cancelled it earlier. The last man standing check is triggering because a new player spawned.
		{
			// The point of all this is so we can use the existing arena instead of making a new one when someone joins.

			%minigame.lastResetTime = getSimTime(); // We'll tell the minigame that it just reset so we don't actually have to reset it.
			%minigame.timeLimitTick(); // Now we restart the tick we cancelled earlier.

			$CA::Paused = 0;
			$CA::Start = getSimTime();
			$CA::GameDelay = 7000 + (clientGroup.getCount() * 450);
			$CA::Loop::MakeSpawn = schedule(33,0,makeNewSpawn,$CA::MS1,$CA::MS2,$CA::MS3,$CA::MS4,$CA::MS5); // Now, we'll start the game instantly!

			return; // Cancel the actual last man standing check so it doesn't reset the minigame.
			// With this, the player can join and begin immediately rather than waiting for the arena to regenerate.
		}

		// Now we'll move on to the actual last man standing stuff.

		// The game-mode uses regular last man standing but we're going to apply a few "hacks" to make it work better for the game-mode.
		if($CA::ClientCount == 1 && clientGroup.getCount() > 1 && !$CA::SoloRoundStarted) // If only one player is active...
		{
			//If only one active player is alive, we'll start a solo round.
			$CA::SoloRoundStarted = 1; // This is to make sure we don't prevent the next last man standing check (For example, if another player joins)
			$CA::SoloRoundMsgSchedule = schedule(1000,0,messageAll,'',"\c5Solo round started because everyone else is dead.");
		}
		else
		{
			cancel($CA::SoloRoundMsgSchedule); // Everyone died, don't show the "solo round started" message.
			parent::checkLastManStanding(%minigame,%b);

			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				if(clientGroup.getObject(%i).player)
				{
					%num++;
					%client = clientGroup.getObject(%i);
				}
			}

			if(%num == 1 && !$CA::Winner)
			{
				$CA::Winner = 1;
				$CA::GameEnded = 1;
				$CA::Score[%client.bl_id]++;
				$CA::Crumble = 0;

				cancel($CA::Loop::Modifier);

				awardRoundEndAchievements(%client);

				echo("Upading bricks destroyed for client" SPC %client.bl_id);
				$CA::ScoreDestroyed[%client.bl_id] = $CA::ScoreDestroyed[%client.bl_id]+%client.player.bricksDestroyed;
				%client.player.bricksDestroyed = 0;

				if($CA::Score[%client.bl_id] != 1)
					%plural = "s";

				messageAll('',"\c3" @ %client.name SPC "\c5has won\c3" SPC $CA::Score[%client.bl_id] SPC "\c5time" @ %plural @ ".");
			}
			else if(%num == 0)
			{
				cancel($CA::Loop::Modifier);
				$CA::GameEnded = 1;
			}
		}
	}

	function GameConnection::spawnPlayer(%client,%b)
	{
		parent::spawnPlayer(%client,%b);
		%client.score = $CA::Score[%client.bl_id];
	}

	function GameConnection::autoAdminCheck(%client,%b,%c,%d,%e,%f,%g)
	{
		%client.HUD = 1;
		return parent::autoAdminCheck(%client,%b,%c,%d,%e,%f,%g);

		//%client.score = $CA::Score[%client.bl_id]; // Untested. Does this work here? (Probably resets upon joining the minigame)
	}

	function GameConnection::sendTrustFailureMessage() { } // To remove the "You cannot modify public bricks" message

	function projectileData::radiusDamage(%projectile,%a,%b,%c,%d,%e,%f,%g) // Achievement: Explosive brick
	{
		%client = %b.client;

		if(%projectile.explosion $= "rocketExplosion")
			%client.player.achievementExplosion = 1; // Mark the player to receive the explosion achievement if they die.

		Parent::radiusDamage(%projectile,%a,%b,%c,%d,%e,%f,%g);
	}

	function onServerDestroyed()
	{
		$CA::HasLoaded = 0;

		if(isObject(MusicData))
		{
			for(%i=0;%i<MusicData.getCount();%i++)
				MusicData.getObject(%i).delete();

			MusicData.delete();
		}
		parent::onServerDestroyed();
	}

	function CALoop()
	{
		cancel($CALoop);

		%crumbleStart = $CA::GameDelay+28000+$CA::ClientCount*17000;
		if(getSimTime() - $CA::Start > %crumbleStart-2000 && !$CA::FTWarn && !$CA::GameEnded && !$CA::Paused) // Original: 145000
		{
			$CA::FTWarn = 1;
			messageAll('',"\c5Uh oh, the arena is getting unstable!");
		}

		if(getSimTime() - $CA::Start > %crumbleStart && !$CA::GameEnded && !$CA::Paused || $CA::Crumble && !$CA::GameEnded)
		{
			$CA::Delay = 280;
			if(brickGroup_888888.getCount() >= 1) // In case there are no bricks left
			{
				for(%i=0;%i<$CA::Layers;%i++)
				{
					%brick = BrickGroup_888888.getObject(getRandom(0,BrickGroup_888888.getCount()-1)); // Fixed by adding "-1"
					if(isObject(%brick) && %brick.getName() !$= "MusicBrick" && !%brick.bombTriggered)
					{
						%brick.setColorFX(3);
						%brick.schedule(1500,destroyBrick); // Removed "fall" sounds for now

					}
				}
			}
		}

		$CALoop = schedule($CA::Delay,0,CALoop);
	}

	function checkVelocity()
	{
		cancel($CA::Loop::Velocity);

		if(getSimTime() - $CA::Start > $CA::GameDelay)
			%gameStarted = 1;

		if(!$CA::GameEnded) // Freeze the timer when the game is over
		{
			$CA::Time = ((getSimTime() - $CA::Start) - $CA::GameDelay)/1000;
			$CA::TimeDisplay = getSubStr(getTimeString($CA::Time),0,4);
		}

		if($CA::TimeDisplay $= "0:0-")
		{
			$CA::TimeDisplay = "0:00";
			commandToAll('centerPrint',"<font:impact:50>\c3Arena size: " @ $CA::ArenaSizeDisplay @ "<br>\c3Brick type: " @ $CA::ArenaBrick @ "<br><br><font:impact:60>\c3" @ mFloor(($CA::GameDelay - (getSimTime() - $CA::Start))/1000)+1,mFloor($CA::GameDelay/1000));
		}

		if(!$CA::RoundStartMessage)
			messageAll('',"\c6Arena size: " @ $CA::ArenaSizeDisplay @ "; Brick type: " @ $CA::ArenaBrick);

		$CA::RoundStartMessage = 1;

		// This is to kill players that try to screw things up with the DLL. (Temporary until I can make it brick-based)
		if(getSimTime()-$CA::LastLagCheck > 2800 && %gameStarted)
		{
			$CA::LastLagCheck = getSimTime(); // Reset the timer
			%lagCheck = 1;
		}

		for(%i=0;%i<clientGroup.getCount();%i++)
		{
			%client = clientGroup.getObject(%i);

			if(isObject(%client.player) && getWord(%client.player.getVelocity(),2) < -40)
			{
				if(%gameStarted) // Make sure the round is actually in progress.
					%client.player.kill();
				else // If it isn't, return the player instead of killing them.
				{
					%client.player.setVelocity("0 0 -1"); // Reset their velocity. If we don't do this, they get stuck in an infinite respawn loop.
					%client.player.setTransform(%client.player.spawnPoint);
				}
			}
			else if(isObject(%client.player) && %lagCheck) // If player is alive and it has been at least one second since last check
			{
				if(%client.player.getTransform() $= %client.player.lastTransform && %client.player.getVelocity() !$= "0 0 0")
				{
					talk("Client appears to be frozen: " @ %client.name @ "; transform=" @ %client.player.getTransform() @ "; last=" @ %client.player.lastTransform);

					%client.player.noIdle = 1;
					$CA::ClientCount--;
					%client.player.kill();
				}
				else
					%client.player.lastTransform = %client.player.getTransform();
			}

			if(%client.debugHUD)
				%hudPrefix = "bd:" SPC %client.player.bricksDestroyed SPC "cc: " @ $CA::ClientCount @ " sk:" SPC %client.player.swordKills @ "<BR>";
			else
				%hudPrefix = "";

			if(%client.HUD && %client.hasSpawnedOnce) // IMPORTANT: Never send bottom prints to clients that are loading. You will break the download system.
				commandToClient(%client,'bottomPrint',%hudPrefix @ "<font:impact:45>\c3" @ $CA::TimeDisplay @ "<just:right>\c3" @ $CA::BrickCount SPC "bricks left",0,1); // Exclude the music brick
		}

		$CA::Loop::Velocity = schedule(10,0,checkVelocity);
	}

	function PlayerNoJet::OnImpact(%this,%player,%col,%vec,%force) // Borrowed this from the Blockheads Ruin X-Mas game-mode
	{
		parent::onImpact(%this, %player, %col, %a, %b, %c);
		if(%force < 4)
			return;
		if(%col.getdatablock() != %this)
			return;
		if(vectornormalize(%vec) !$= "0 0 1")
			return;

		%force = mFloor(%force);

		if(getSimTime() - $CA::Start > $CA::GameDelay)
		{
			awardAchievement(%player.client,1); // Award "Mario" achievement

			if(%player.achievementMario2 && %player.achievementMarioPlayer !$= %col.getID()) // If they step on a different player's head without touching a brick, award the "Mario II" achievement.
				awardAchievement(%player.client,2);

			%player.achievementMario2 = 1;
			%player.achievementMarioPlayer = %col.getID();
		}

		//centerprint(%col.client, "<bitmap:base/client/ui/ci/crater> \c0Stomp \c6from \c0" @ %player.client.name @ " \c6at \c0" @ %force @ " \c6ft/sec. " @ %vec, 4);
		//centerprint(%player.client, "<bitmap:base/client/ui/ci/crater> \c0Stomp \c6to \c0" @ %col.client.name @ " \c6at \c0" @ %force @ " \c6ft/sec. " @ %vec, 4);
	}

	function serverCmdUseTool(%client,%a)
	{
		Parent::serverCmdUseTool(%client,%a);
		%client.player.noIdle = 1;
	}

	function armor::onDisabled(%damage,%player,%a)
	{
		//commandToClient(%player.client,'centerPrint',"You died!<br>Bricks destroyed: " @ %player.bricksDestroyed);

		%client = %player.client;

		if($CA::Time <= 10 && !%client.player.noIdle)
		{
			$CA::ClientCount--; // If a player dies within the first ten seconds, exclude them from the "unstable" timer.
			//%client.consecutiveDeaths++;

			//if(%client.consecutiveDeaths >= 2)
			//{
			//	%client.consecutiveDeaths = 0;
			//	//"Click to respawn" code here
			//	%client.awaitingSpawnPrompt = 1;
			//	echo(%client.name SPC "(click to respawn)");
			//}
		}
		else if(!$CA::SoloRoundStarted && !$CA::GameEnded) // Don't count the death if it's a solo round or the game is over.
		{
			echo("Upading bricks destroyed for client" SPC %client.bl_id);
			$CA::ScoreDestroyed[%client.bl_id] = $CA::ScoreDestroyed[%client.bl_id]+%client.player.bricksDestroyed; // Add bricks destroyed to their stats
			$CA::ScoreLoss[%client.bl_id]++; // Counts as a loss
		}

		if(!%player.bricksDestroyed)
			%player.bricksDestroyed = 0;

		messageClient(%player.client,'',"\You died!<br>Bricks destroyed: \c6" @ %player.bricksDestroyed);

		if(%player.achievementExplosion) // If a player died after getting hit by an explosion, give them the achievement.
			awardAchievement(%player.client,0);

		Parent::onDisabled(%damage,%player,%a);
		//talk("rip" SPC %player.client.name);
	}

	function GameConnection::onDeath(%target,%projectile,%client,%d,%e,%f)
	{
		if(%target != %client)
		{
			//echo("swordkill" SPC %client SPC ">" SPC %target);
			$CA::ScoreSwordKills[%client.bl_id]++;
			%client.player.swordKills++;
			if(%client.player.swordKills == 3)
				awardAchievement(%client,4); // Award the "Aggressive" achievement
		}

		Parent::onDeath(%target,%projectile,%client,%d,%e,%f);
	}

	function ItemData::onAdd(%a,%item) // When an item is dropped, we'll wait 3 seconds and check if it is falling. If so, we award the "Peace Treaty" achievement. See achievements.cs for the CA_checkItemVelocity function.
	{
		schedule(3000,0,CA_checkItemVelocity,%item);
		Parent::onAdd(%a,%item);
	}

	function Player::activateStuff(%player)
	{
		Parent::ActivateStuff(%player);
	}

	function serverCmdPlantBrick(%client)
	{
		// Vote
		Parent::serverCmdPlantBrick(%client);
	}
};
activatePackage(CrumblingArenaPackage);
CALoop();
checkVelocity();
