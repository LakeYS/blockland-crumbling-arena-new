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
	
	%filenameB = "config/server/CrumbleArena/achievements.cs";
	if(isFile(%filenameB))
		exec(%filenameB);
	
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

function getBrick(%brick)
{
	//if(getRandom(1,150) == 1) // Rare tree brick
	//{
	//	$CA::Trees = 1;
	//	if($CA::XmasStuff)
	//		return "brickChristmasTreeData";
	//	else
	//		return "brickPineTreeData";
	//}
	//else
	//	$CA::Trees = 0;

	switch(%brick)
	{
		case 0:
			return "brick4xCubeData";

		case 1:
			return "brick4x4Data";

		case 2:
			return "brick4x8Data";

		case 3:
			return "brick4x4FData";

		case 4:
			return "brick2x4Data";

		case 5:
			return "brick4xCubeData"; // Extra chance for 4x cubes

		case 6:
			return "brick2x4x3Data"; // Extra chance for 2x4x3

		case 7:
			return "brick2x4x3Data";

		case 8:
			return "brick2x2Data";

		case 9:
			return "brick2xCubeData";
	}
}

function getBrickSize(%brick)
{
	return %brick.brickSizeX/2 SPC %brick.brickSizeY/2 SPC %brick.brickSizeZ/5;
}

function buildArena()
{
	echo("SERVER: Building arena...");

	%arenaSize = 15 + ClientGroup.getCount();
	if(%arenaSize > 24)
		%arenaSize = 24;
	
	%arenaHeight = getRandom(2,9);
	$CA::Layers = %arenaHeight;
	
	$CA::BrickDatablock = getBrick(getRandom(0,9));
	%brickSize = getBrickSize($CA::BrickDatablock);

	//may help with the glitched bricks
	%arenaOffsetX = getRandom(-700,700);
	%arenaOffsetY = getRandom(-700,700);

	echo("SIZE=" @ %arenaSize SPC "HEIGHT=" @ %arenaHeight SPC "BRICK=" @ $CA::BrickDatablock);
	
	//talk(%arenaSize*getWord(%brickSize,1)*2);
	
	$CA::ArenaSizeDisplay = %arenaSize @ "x" @ %arenaSize @ "x" @ %arenaHeight @ %f;
	$CA::ArenaBrick = $CA::BrickDatablock.uiName;
	$CA::ArenaIcon = $CA::BrickDatablock.iconName;

	for(%z=0;%z<%arenaHeight;%z++)
	{
		%positionZ = 200 + getWord(%brickSize,2)*%z;

		for(%x=0;%x<%arenaSize;%x++)
		{
			%positionX = %arenaOffsetX + (getWord(%brickSize,0)*%x);

			for(%y=0;%y<%arenaSize;%y++)
			{
				%positionY = %arenaOffsetY + (getWord(%brickSize,1)*%y);
				%brick = new fxDTSBrick()
				{
					angleID = 0;
					client = 0;
					colorFxID = 0;
					colorID = %z;
					dataBlock = $CA::BrickDatablock;
					isBasePlate = 0;
					isPlanted = 1;
					position = %positionX SPC %positionY SPC %positionZ;
					printID = 0;
					rotation = "0 0 0";
					scale = "1 1 1";
					shapeFxID = 0;
					stackBL_ID = "-1";
					numEvents = 1;
				};
				
				//if(%z == %arenaHeight-1)
				//{
				//	if(%y == %arenaSize-1 || %y == 0 || %x == %arenaSize-1 || %x == 0)
				//		%brick.setColor(%arenaHeight-2);
				//}
				
				if(getRandom(1,2000) == 1) //
				{
					%brick.setEmitter("AdminWandEmitterB");
					%brick.setColorFX(5);
					%brick.isBombBrick = 1;
				}
				
				%brick.plant();
				%brick.setTrusted(1);
				BrickGroup_888888.add(%brick);
				%brick.playSound(brickPlantSound);

				if(!%firstBrick)
					%firstBrick = %brick;
				
			}
		}
	}

	if(!%lastBrick)
		%lastBrick = %brick;

	%middle = (getWord(%firstBrick.getPosition(),0) + getWord(%lastBrick.getPosition(),0))/2 SPC (getWord(%firstBrick.getPosition(),1) + getWord(%lastBrick.getPosition(),1))/2 SPC %positionZ + 50;
	
	if($CA::Music)
	{
		%musicBrick = new fxDTSBrick(MusicBrick)
		{
			angleID = 0;
			client = 0;
			colorFxID = 0;
			colorID = 0;
			dataBlock = "brick15xMusicData";
			isBasePlate = 0;
			isPlanted = 1;
			position = %middle;
			printID = 0;
			rotation = "0 0 0";
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = "-1";
			numEvents = 1;
		};
		%musicBrick.plant();
		%musicBrick.setTrusted(1);
		BrickGroup_888888.add(%musicBrick);
		%musicBrick.playSound(brickPlantSound);
		%musicBrick.setMusic(MusicData.getObject(getRandom(0,MusicData.getCount()-1)).music.getID());
		%musicBrick.setRendering(0);
		%musicBrick.setColliding(0);
	}

	$CA::GameDelay = 7000 + (clientGroup.getCount() * 450);
	
	$CA::Loop::MakeSpawn = schedule(33,0,makeNewSpawn,%positionX,%positionY,%positionZ + 20,%firstBrick,%lastBrick);
}

function makeNewSpawn(%x,%y,%z,%b1,%b2)
{
	%firstPos = %b1.getPosition();
	%lastPos = %b2.getPosition();

	for(%i=0;%i<clientGroup.getCount();%i++)
	{
		%obj = ClientGroup.getObject(%i);

		if(%obj.player)
		{
			//%obj.player.delete();
			%obj.spawnPlayer();
			%obj.player.addVelocity("0 0 -1");

			%obj.player.tool[0] = "";
			messageClient(%obj,'MsgItemPickup','',0,0);

			%obj.player.setTransform(getRandom(getWord(%firstPos,0)+15,getWord(%lastPos,0)-15) SPC getRandom(getWord(%firstPos,1)+15,getWord(%lastPos,1)-15) SPC %z);
		}
	}
	
	cancel($CA::Loop::Modifier);
	$CA::Loop::Modifier = schedule($CA::GameDelay,0,doRoundModifier,getRandom(1,13)); //getRandom(1,13)
	//schedule($CA::GameDelay,0,doRoundModifier,7);
	
	$CA::BrickCount = getBrickCount();
	$CA::ClientCount = clientGroup.getCount();
	
	for(%i=0;%i<clientGroup.getCount();%i++) // We want to exclude people that are loading from the count.
	{
		%client = clientGroup.getObject(%i);
		
		if(!%client.minigame) // If they aren't in the minigame, they're still loading.
			$CA::ClientCount--;
	}
	
	$CA::Start = getSimTime();
	
	if($CA::ClientCount == 1)
		$CA::SoloRoundStarted = 1;
}

function doRoundModifier(%which)
{
	//talk("doRoundModifier" SPC %which);
	centerPrintAll("<font:impact:60>\c3BEGIN!",5);
	$CA::RoundModifierID = %which;
	
	if($CA::Trees)
		%which = 5; // Use horses if the tree event is happening
	//else if(getRandom(1,100) == 1 && $CA::ClientCount > 1) // Guns (very rare)
	//{
	//	for(%i=0;%i<clientGroup.getCount();%i++)
	//	{
	//		%obj = clientGroup.getObject(%i);
    //
	//		if(isObject(%obj.player))
	//		{
	//			%obj.player.tool[0] = GunItem.getID();
	//			messageClient(%obj,'MsgItemPickup','',0,GunItem.getID());
	//		}
	//	}
	//	centerPrintAll("<font:impact:64>\c3GUNS!",5);
	//	return;
	//}
	
	switch(%which)
	{
		case 1: //Pushbrooms
			if($CA::ClientCount > 1)
			{
				for(%i=0;%i<clientGroup.getCount();%i++)
				{
					%obj = clientGroup.getObject(%i);
					
					if(isObject(%obj.player))
					{
						%obj.player.tool[0] = PushBroomItem.getID();
						messageClient(%obj,'MsgItemPickup','',0,PushBroomItem.getID());
					}
				}
				centerPrintAll("<font:impact:60>\c3Pushbrooms!",5);
			}

		case 2: //Huge
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
					%obj.player.setPlayerScale("1.75 1.75 1.75");
			}
			centerPrintAll("<font:impact:60>\c3Giants!",5);

		case 3: //Auto crumble
			centerPrintAll("<font:impact:60>\c3The arena is unstable!",5);
			$CA::Crumble = 1;
			$CA::FTWarn = 1;

		case 4: //Small playerscale
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
					%obj.player.setPlayerScale("0.6 0.6 0.6");
			}
			centerPrintAll("<font:impact:60>\c3Haha, alright then shortstuffs.",5);

		case 5: //Horses
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
					%obj.player.setDatablock(HorseArmor);
			}
			centerPrintAll("<font:impact:60>\c3Horses, because horses.",5);

		case 6: //Swords
			if($CA::ClientCount > 1)
			{
				for(%i=0;%i<clientGroup.getCount();%i++)
				{
					%obj = clientGroup.getObject(%i);
					
					if(isObject(%obj.player))
					{
						%obj.player.tool[0] = SwordItem.getID();
						messageClient(%obj,'MsgItemPickup','',0,SwordItem.getID());
					}
				}
				centerPrintAll("<font:impact:60>\c3Swords!",5);
			}
		case 7: //Slow
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
				{
					%d = %obj.player.dataBlock;
					
					%obj.player.setMaxForwardSpeed(%d.maxForwardSpeed/2);
					%obj.player.setMaxBackwardSpeed(%d.maxBackwardSpeed/2);
					%obj.player.setMaxSideSpeed(%d.maxSideSpeed/2);
					
					%obj.player.setMaxCrouchForwardSpeed(%d.maxForwardCrouchSpeed/2);
					%obj.player.setMaxCrouchBackwardSpeed(%d.maxBackwardCrouchSpeed/2);
					%obj.player.setMaxCrouchSideSpeed(%d.maxSideCrouchSpeed/2);
				}
			}
			centerPrintAll("<font:impact:60>\c3Slowpokes!",5);
		case 8: //Fast
		for(%i=0;%i<clientGroup.getCount();%i++)
		{
			%obj = clientGroup.getObject(%i);
			
			if(isObject(%obj.player))
			{
				%d = %obj.player.dataBlock;
				
				%obj.player.setMaxForwardSpeed(%d.maxForwardSpeed*1.8);
				%obj.player.setMaxBackwardSpeed(%d.maxBackwardSpeed*1.8);
				%obj.player.setMaxSideSpeed(%d.maxSideSpeed*1.8);
				
				%obj.player.setMaxCrouchForwardSpeed(%d.maxForwardCrouchSpeed*1.8);
				%obj.player.setMaxCrouchBackwardSpeed(%d.maxBackwardCrouchSpeed*1.8);
				%obj.player.setMaxCrouchSideSpeed(%d.maxSideCrouchSpeed*1.8);
			}
		}
		centerPrintAll("<font:impact:60>\c3Gotta go fast!",5);
		case 9: //Low gravity
			CAGravityZone.gravityMod = 0.6;
			CAGravityZone.sendUpdate(); // Fixed?
			centerPrintAll("<font:impact:60>\c3SPACE!",5);
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
	
	if(!%wins)
		%wins = 0;
	
	if(!%loss)
		%loss = 0;
	
	if(!%bricks)
		%bricks = 0;
	
	messageClient(%client,'CAStats',"\c5You have won \c3" @ %wins @ "\c5 times. You have died \c3" @ %loss @ "\c5 times. You have destroyed a total of \c3" @ %bricks @ "\c5 bricks!");
}

function serverCmdAchievements(%client) // WIP
{
	%msg = "\c5(Requires at least \c33\c5 active players)";
	messageClient(%client,'CAAchievementList',"\c3Winner!\c5 - Win a game against four players or more." SPC %msg);
	messageClient(%client,'CAAchievementList',"\c3Cautious\c5 - Win a normal round by only touching bricks on the top layer." SPC %msg);
	messageClient(%client,'CAAchievementList',"\c3Nerf This!\c5 - Die from an explosive brick.");
	messageClient(%client,'CAAchievementList',"\c3Mario\c5 - Jump on someone's head!");
	messageClient(%client,'CAAchievementList',"\c3Peace Treaty\c5 - Drop your weapon in a sword or broom round.");
	messageClient(%client,'CAAchievementList',"\c3WEEEEEEEEE!\c5 - Survive an explosive brick.");
	messageClient(%client,'',"\c3Press Page Up and Page Down to scroll in chat.");

}

function awardRoundEndAchievements(%client)
{
	%blid = %client.bl_id;
	if($CA::Score[%blid] >= 1 && $CA::ClientCount > 2 && !$CA::AchievementWinner[%blid])
	{
		messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Winner!\c5 achievement!");
		$CA::AchievementWinner[%blid] = 1;
	}
	
	switch($CA::RoundModifierID)
	{
		case 1: //Pushbrooms
			if($CA::ClientCount > 3 && !$CA::AchievementBrooms[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Broom Fighter\c5 achievement!");
				$CA::AchievementBrooms[%blid] = 1;
			}
			return;
		case 2: //Huge
			if($CA::ClientCount > 3 && !$CA::AchievementGiant[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Giant\c5 achievement!");
				$CA::AchievementGiant[%blid] = 1;
			}
			return;
		case 3: //Auto crumble
			if($CA::ClientCount > 3 && !$CA::AchievementUnstable[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Askew\c5 achievement!");
				$CA::AchievementUnstable[%blid] = 1;
			}
			return;
		case 4: //Small playerscale
			if($CA::ClientCount > 3 && !$CA::AchievementTiny[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Ant\c5 achievement!");
				$CA::AchievementTiny[%blid] = 1;
			}
			return;
		case 5: //Horses
			if($CA::ClientCount > 3 && !$CA::AchievementHorse[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Horse\c5 achievement!");
				$CA::AchievementHorse[%blid] = 1;
			}
			return;
		case 6: //Swords
			if($CA::ClientCount > 3 && !$CA::AchievementSwords[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Sword Fighter\c5 achievement!");
				$CA::AchievementSwords[%blid] = 1;
			}
			return;
		case 7: //Slow
			if($CA::ClientCount > 3 && !$CA::AchievementSlow[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Snail\c5 achievement!");
				$CA::AchievementSlow[%blid] = 1;
			}
			return;
		case 8: //Fast
			if($CA::ClientCount > 3 && !$CA::AchievementFast[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Hedgehog\c5 achievement!");
				$CA::AchievementFast[%blid] = 1;
			}
			return;
		case 9: //Low gravity
			if($CA::ClientCount > 3 && !$CA::AchievementSpace[%blid])
			{
				//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Astronaut\c5 achievement!");
				$CA::AchievementSpace[%blid] = 1;
			}
			return;
	}
	
	if(isObject(%player) && !%player.achievementNoCautious && $CA::ClientCount > 2 && !$CA::AchievementCautious[%blid])
	{
		messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Cautious\c5 achievement!");
		$CA::AchievementCautious[%blid] = 1;
	}
	
	if($CA::ClientCount > 3 && !$CA::AchievementNormal[%blid])
	{
		//messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Vanilla\c5 achievement!");
		$CA::AchievementNormal[%blid] = 1;
	}
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
	function fxDTSBrick::destroyBrick(%this,%force,%sound,%player)
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
			//%this.schedule(125,fakeKillBrick);
			%this.setEmitter();
			%this.setLight();
			%this.setColor(%this.colorID+10); // Make the brick transparent
			if(%sound !$= "")
				%this.playSound(%sound);
			%this.schedule(125,fakeKillBrick,"0 0 0",3);
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
				
			%this.destroyBrick();
		}
		else
			Parent::onFakeDeath(%this);
	}
	
	function fxDtsBrick::onPlayerTouch(%this,%player)
	{		
		if(%this && !%player.client.noCrumble)
		{
			if(%this.colorID+1 != $CA::Layers)
				%player.achievementNoCautious = 1;

			if(%player.achievementExplosion)
			{
				%player.achievementExplosion = 0; // If the player is touching bricks, they have not died from an explosion.
				if(!$CA::AchievementExplosionSurvive[%player.client.bl_id])
				{
					messageAll('',"\c3" @ %player.client.name @ "\c5 has earned the \c3WEEEEEEEEE!\c5 achievement!");
					$CA::AchievementExplosionSurvive[%player.client.bl_id] = 1;
				}
			}
			
			%this.destroyBrick(0,"brickStep0",%player);
		}
	}

	function MinigameSO::reset(%a,%b,%c,%d,%e,%f,%g)
	{
		BrickGroup_888888.deleteAll();
		buildArena();

		$CA::FallingTiles = 0;
		$CA::FTWarn = 0;
		$CA::Start = getSimTime();
		$CA::Winner = 0;
		$CA::GameEnded = 0;
		$CA::Delay = 3000;
		$CA::Crumble = 0;
		$CA::RoundStartMessage = 0;
		$CA::SoloRoundStarted = 0;

		CAGravityZone.gravityMod = 1;
		CAGravityZone.sendUpdate();
		//CAGravityZone.deactivate();

		export("$CA::Score*","config/server/CrumbleArena/scores.cs");
		export("$CA::Achievement*","config/server/CrumbleArena/achievements.cs");

		parent::reset(%a,%b,%c,%d,%e,%f,%g);
	}

	function MinigameSO::checkLastManStanding(%a,%b)
	{
		if($CA::ClientCount == 1 && clientGroup.getCount() > 1 && !$CA::SoloRoundStarted) // In case only one player is active
		{
			for(%i=0;%i<clientGroup.getCount();%i++) // Probably redundant now that $CA::SoloRoundStarted is implemented
			{
				if(clientGroup.getObject(%i).player)
				{
					%num++;
					%client = clientGroup.getObject(%i);
				}
			}
			
			if(!%num) // Probably redundant now that $CA::SoloRoundStarted is implemented
			{
				cancel($CA::SoloRoundMsgSchedule);
				parent::checkLastManStanding(%a,%b);
			}
			else
			{
				$CA::SoloRoundStarted = 1; // This is to make sure we don't prevent the next last man standing check (For example, if another player joins)
				$CA::SoloRoundMsgSchedule = schedule(1000,0,messageAll,'',"\c5Solo round started because everyone is dead.");
			}
		}
		else
		{
			cancel($CA::SoloRoundMsgSchedule);
			parent::checkLastManStanding(%a,%b);
			
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
	}
	
	function GameConnection::sendTrustFailureMessage() { } // To remove the "You cannot modify public bricks" message
	
	function projectileData::radiusDamage(%projectile,%a,%b,%c,%d,%e,%f,%g) // Achievement: Explosive brick
	{
		%client = %b.client;
		if(%projectile.explosion $= "rocketExplosion" && !$CA::AchievementExplosion[%client.bl_id])
			%client.player.achievementExplosion = 1; // Mark the player to receive the explosion achievement if they die. If they survive (aka they touch a brick)
		
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
		if(getSimTime() - $CA::Start > %crumbleStart-2000 && !$CA::FTWarn && !$CA::GameEnded) // Original: 145000
		{
			$CA::FTWarn = 1;
			messageAll('',"\c5Uh oh, the arena is getting unstable!");
		}

		if(getSimTime() - $CA::Start > %crumbleStart && !$CA::GameEnded || $CA::Crumble && !$CA::GameEnded)
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
		if(getSimTime()-$CA::LastLagCheck > 2800 && getSimTime() - $CA::Start > $CA::GameDelay && getSimTime() - $CA::Start > $CA::GameDelay)
		{
			$CA::LastLagCheck = getSimTime(); // Reset the timer
			%lagCheck = 1;
		}
		
		for(%i=0;%i<clientGroup.getCount();%i++)
		{
			%obj = clientGroup.getObject(%i);
			if(isObject(%obj.player) && getWord(%obj.player.getVelocity(),2) < -40)
			{
				if($CA::Time <= 9 && !%obj.player.noIdle)
					$CA::ClientCount--; // If a player dies within the first ten seconds, exclude them from the "unstable" timer.
				else
				{
					$CA::ScoreDestroyed[%obj.bl_id] = $CA::ScoreDestroyed[%obj.bl_id]+%obj.player.bricksDestroyed; // Add bricks destroyed to their stats
					$CA::ScoreLoss[%obj.bl_id]++; // Counts as a loss
				}
				%obj.player.kill();
			}
			else if(isObject(%obj.player) && %lagCheck) // If player is alive and it has been at least one second since last check
			{
				if(%obj.player.getTransform() $= %obj.player.lastTransform && %obj.player.getVelocity() !$= "0 0 0")
				{
					talk("Client appears to be frozen: " @ %obj.name @ "; transform=" @ %obj.player.getTransform() @ "; last=" @ %obj.player.lastTransform);
					$CA::ClientCount--;
					%obj.player.kill();
				}
				else
					%obj.player.lastTransform = %obj.player.getTransform();
			}
			
			if(%obj.debugHUD)
				%hudPrefix = "bd:" SPC %obj.player.bricksDestroyed SPC "cc: " @ $CA::ClientCount @ "<BR>";
			
			if(%obj.HUD && %obj.hasSpawnedOnce) // IMPORTANT: Never send bottom prints to clients that are loading. You will break the download system.
				commandToClient(%obj,'bottomPrint',%hudPrefix @ "<font:impact:45>\c3" @ $CA::TimeDisplay @ "<just:right>\c3" @ $CA::BrickCount SPC "bricks left",0,1); // Exclude the music brick
		}
		
		$CA::Loop::Velocity = schedule(10,0,checkVelocity);
	}
	
	function PlayerNoJet::OnImpact(%this,%obj,%col,%vec,%force) // Borrowed this from the Blockheads Ruin X-Mas game-mode
	{
		parent::onImpact(%this, %obj, %col, %a, %b, %c);
		if(%force < 4)
			return;
		if(%col.getdatablock() != %this)
			return;
		if(vectornormalize(%vec) !$= "0 0 1")
			return;
		
		%force = mFloor(%force);
		
		if(getSimTime() - $CA::Start > $CA::GameDelay && !$CA::AchievementStomp[%obj.client.bl_id])
		{
			messageAll('',"\c3" @ %obj.client.name @ "\c5 has earned the \c3Mario\c5 achievement!");
			$CA::AchievementStomp[%obj.client.bl_id] = 1;
		}
		
		//TODO: Implement Mario II
		
		//centerprint(%col.client, "<bitmap:base/client/ui/ci/crater> \c0Stomp \c6from \c0" @ %obj.client.name @ " \c6at \c0" @ %force @ " \c6ft/sec. " @ %vec, 4);
		//centerprint(%obj.client, "<bitmap:base/client/ui/ci/crater> \c0Stomp \c6to \c0" @ %col.client.name @ " \c6at \c0" @ %force @ " \c6ft/sec. " @ %vec, 4);
	}
	
	function serverCmdDropTool(%client,%a)
	{
		Parent::serverCmdDropTool(%client,%a);
		
		if($CA::RoundModifierID == 6 || $CA::RoundModifierID == 1)
		{
			if(getSimTime() - $CA::Start > $CA::GameDelay && $CA::ClientCount > 1 && !$CA::AchievementDrop[%client.bl_id])
			{
				messageAll('',"\c3" @ %client.name @ "\c5 has earned the \c3Peace Treaty\c5 achievement!");
				$CA::AchievementDrop[%client.bl_id] = 1;
			}
		}
	}
	
	function serverCmdUseTool(%client,%a)
	{
		Parent::serverCmdUseTool(%client,%a);
		%client.player.noIdle = 1;
	}
	
	function armor::onDisabled(%damage,%player,%a)
	{
		//commandToClient(%player.client,'centerPrint',"You died!<br>Bricks destroyed: " @ %player.bricksDestroyed);
		messageClient(%player.client,'',"\You died!<br>Bricks destroyed: \c6" @ %player.bricksDestroyed);
		
		if(%player.achievementExplosion && !$CA::AchievementExplosion[%player.client.bl_id]) // If a player died after getting hit by an explosion, give them the achievement.
		{
			messageAll('',"\c3" @ %player.client.name @ "\c5 has earned the \c3Nerf This!\c5 achievement!");
			$CA::AchievementExplosion[%player.client.bl_id] = 1;
		}
		
		Parent::onDisabled(%damage,%player,%a);
		//talk("rip" SPC %player.client.name);
	}
};
activatePackage(CrumblingArenaPackage);
CALoop();
checkVelocity();