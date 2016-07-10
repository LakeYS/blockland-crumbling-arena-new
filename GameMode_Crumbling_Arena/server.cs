//for(%i = 1; %i <= 1000; %i++) if(getRandom(1,8000) == 1) echo(boomm);

if(getSubStr(getDateTime(),0,2) == 12)
{
	$CA::XmasStuff = 1;
	exec("Add-Ons/Brick_Christmas_Tree/server.cs");
}

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

datablock fxDTSBrickData(brick15xMusicData : brickMusicData)
{
	uiName = "Music (15x Range)";
	musicRange = 450;
	musicDescription = Audio15xMusicLooping3d;
};

datablock PlayerData(PlayerFrozenArmor : PlayerStandardArmor)
{
	runForce = 0;
	runEnergyDrain = 0;
	minRunEnergy = 0;
	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;
	horizResistFactor = 1.0;
	thirdPersonOnly = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	jumpForce = 0;
	jumpEnergyDrain = 0;
	minJumpEnergy = 0;
	jumpDelay = 0;

	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
};

if(!$CA::HasLoaded)
{
	$CA::HasLoaded = 1;
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
	if(getRandom(1,150) == 1) // Rare tree brick
	{
		$CA::Trees = 1;
		if($CA::XmasStuff)
			return "brickChristmasTreeData";
		else
			return "brickPineTreeData";
	}
	else
		$CA::Trees = 0;

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
			return "brick8x8Data";

		case 6:
			return "brick8x8FData";

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

//function placeArenaBrick(

function buildArena()
{
	echo("SERVER: Building arena...");

	%arenaSize = 15 + ClientGroup.getCount();
	%arenaHeight = getRandom(2,9);
	$CA::Layers = %arenaHeight;

	%brickDatablock = getBrick(getRandom(0,9));
	%brickSize = getBrickSize(%brickDatablock);

	//may help with the glitched bricks
	%arenaOffsetX = getRandom(-700,700);
	%arenaOffsetY = getRandom(-700,700);

	echo("SIZE=" @ %arenaSize SPC "HEIGHT=" @ %arenaHeight SPC "BRICK=" @ %brickDatablock);
	
	//talk(%arenaSize*getWord(%brickSize,1)*2);
	
	$CA::ArenaSizeDisplay = %arenaSize @ "x" @ %arenaSize @ "x" @ %arenaHeight @ %f;
	$CA::ArenaBrick = %brickDatablock.uiName;
	$CA::ArenaIcon = %brickDatablock.iconName;

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
					dataBlock = %brickDatablock;
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
				
				if(getRandom(1,1500) == 1) 
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

			%obj.player.delete();
			%obj.spawnPlayer();

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
	$CA::Start = getSimTime();
}

function doRoundModifier(%which)
{
	//talk("doRoundModifier" SPC %which);
	centerPrintAll("<font:impact:50>\c3BEGIN!",5);
	
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
				centerPrintAll("<font:impact:50>\c3Pushbrooms!",5);
			}

		case 2: //Huge
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
					%obj.player.setPlayerScale("1.75 1.75 1.75");
			}
			centerPrintAll("<font:impact:50>\c3Giants!",5);

		case 3: //Auto crumble
			centerPrintAll("<font:impact:50>\c3The arena is unstable!",5);
			$CA::Crumble = 1;
			$CA::FTWarn = 1;

		case 4: //Small playerscale
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
					%obj.player.setPlayerScale("0.6 0.6 0.6");
			}
			centerPrintAll("<font:impact:50>\c3Haha, alright then shortstuffs.",5);

		case 5: //Horses
			for(%i=0;%i<clientGroup.getCount();%i++)
			{
				%obj = clientGroup.getObject(%i);
				
				if(isObject(%obj.player))
					%obj.player.setDatablock(HorseArmor);
			}
			centerPrintAll("<font:impact:50>\c3Horses, because horses.",5);

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
				centerPrintAll("<font:impact:50>\c3Swords!",5);
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
			centerPrintAll("<font:impact:50>\c3Slowpokes!",5);
		case 8: //Fast
		for(%i=0;%i<clientGroup.getCount();%i++)
		{
			%obj = clientGroup.getObject(%i);
			
			if(isObject(%obj.player))
			{
				%d = %obj.player.dataBlock;
				
				%obj.player.setMaxForwardSpeed(%d.maxForwardSpeed*2);
				%obj.player.setMaxBackwardSpeed(%d.maxBackwardSpeed*2);
				%obj.player.setMaxSideSpeed(%d.maxSideSpeed*2);
				
				%obj.player.setMaxCrouchForwardSpeed(%d.maxForwardCrouchSpeed*2);
				%obj.player.setMaxCrouchBackwardSpeed(%d.maxBackwardCrouchSpeed*2);
				%obj.player.setMaxCrouchSideSpeed(%d.maxSideCrouchSpeed*2);
			}
		}
		centerPrintAll("<font:impact:50>\c3Gotta go fast!",5);
		case 9: //Low gravity
			CAGravityZone.gravityMod = 0.6;
			CAGravityZone.sendUpdate(); // Fixed?
			centerPrintAll("<font:impact:50>\c3SPACE!",5);
	}
}

function serverCmdToggleHud(%client)
{
	if(%client.HUD)
		%client.HUD = 0;
	else
		%client.HUD = 1;
}

function serverCmdToggleMusic(%client)
{
	if(%client.bl_id == 999999 || %client.bl_id == getNumKeyID())
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
	%bricks = $CA::ScoreBricks[%blid];
	
	if(!%wins)
		%wins = 0;
	
	if(!%loss)
		%loss = 0;
	
	if(!%bricks)
		%bricks = 0;
	
	messageClient(%client,'CAStats',"\c0You have won \c6" @ %wins @ "\c0 times. You have died \c6" @ %loss @ "\c0 times. You have destroyed a total of \c6" @ %bricks @ "\c0 bricks!");
}

deactivatePackage("CrumblingArenaPackage");
package CrumblingArenaPackage
{
	function fxDTSBrick::destroyBrick(%this,%force)
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
				%this.schedule(3500,spawnExplosion,rocketLauncherProjectile,3);
				%this.schedule(3501,destroyBrick,1); // In case the brick mysteriously doesn't explode
				return;
			}
			$CA::BrickCount--;
			%this.crumbled = 1;
			//%this.schedule(125,fakeKillBrick);
			%this.setEmitter();
			%this.setColor(%this.colorID+10); // Make the brick transparent
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
			%this.destroyBrick();
			%player.bricksDestroyed++;
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

		CAGravityZone.gravityMod = 1;
		CAGravityZone.sendUpdate();
		//CAGravityZone.deactivate();

		export("$CA::Score*","config/server/CrumbleArena/scores.cs");

		parent::reset(%a,%b,%c,%d,%e,%f,%g);
	}

	function MinigameSO::checkLastManStanding(%a,%b)
	{
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
			
			$CA::ScoreBricks[%client.bl_id] = $CA::ScoreBricks[%client.bl_id]+%client.player.bricksDestroyed;
			%client.player.bricksDestroyed = 0;
			
			if($CA::Score[%client.bl_id] != 1)
				%plural = "s";
			
			messageAll('',"\c3" @ %client.name SPC "\c5has won\c3" SPC $CA::Score[%client.bl_id] SPC "\c5time" @ %plural @ ".");
		}
		else if(%num == 0)
			$CA::GameEnded = 1;
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
		if(isEventPending($CALoop))
			talk("fuck (CALoop)");
		
		cancel($CALoop);
		
		%crumbleStart = $CA::GameDelay+20000+$CA::ClientCount*18000;
		if(getSimTime() - $CA::Start > %crumbleStart-2000 && !$CA::FTWarn && !$CA::GameEnded) // Original: 145000
		{
			$CA::FTWarn = 1;
			messageAll('',"\c5Uh oh, the arena is getting unstable!");
		}

		if(getSimTime() - $CA::Start > %crumbleStart && !$CA::GameEnded || $CA::Crumble && !$CA::GameEnded)
		{
			$CA::Delay = 280;
			if(brickGroup_888888.getCount() >= 0) // In case there are no bricks left
			{
				for(%i=0;%i<$CA::Layers;%i++)
				{
					%brick = BrickGroup_888888.getObject(getRandom(0,BrickGroup_888888.getCount()-1)); // Fixed by adding "-1"
					if(isObject(%brick) && %brick.getName() !$= "MusicBrick")
					{
						%brick.setColorFX(3);
						%brick.schedule(1500,destroyBrick);

					}
				}
			}
		}

		$CALoop = schedule($CA::Delay,0,CALoop);
	}

	function checkVelocity()
	{
		if(isEventPending($CA::Loop::Velocity))
			talk("fuck (checkVelocity)");
		
		cancel($CA::Loop::Velocity);
		
		if(!$CA::GameEnded) // Freeze the timer when the game is over
		{			
			$CA::Time = ((getSimTime() - $CA::Start) - $CA::GameDelay)/1000;
			$CA::TimeDisplay = getSubStr(getTimeString($CA::Time),0,4);
		}
		
		if($CA::TimeDisplay $= "0:0-")
		{
			$CA::TimeDisplay = "0:00";
			commandToAll('centerPrint',"<font:impact:34>\c3Arena size: " @ $CA::ArenaSizeDisplay @ "<br>\c3Brick type: " @ $CA::ArenaBrick @ "<br><br><font:impact:60>\c3" @ mFloor(($CA::GameDelay - (getSimTime() - $CA::Start))/1000),mFloor($CA::GameDelay/1000));
		}
		
		if(!$CA::RoundStartMessage)
			messageAll('',"\c6Arena size: " @ $CA::ArenaSizeDisplay @ "; Brick type: " @ $CA::ArenaBrick);
		
		$CA::RoundStartMessage = 1;
		
		// This is to kill players that try to screw things up with the DLL. (Temporary until I can make it brick-based)
		if(getSimTime()-$CA::LastLagCheck > 1000)
		{
			$CA::LastLagCheck = getSimTime(); // Reset the timer
			%lagCheck = 1;
		}
		
		for(%i=0;%i<clientGroup.getCount();%i++)
		{
			%obj = clientGroup.getObject(%i);
			if(isObject(%obj.player) && getWord(%obj.player.getVelocity(),2) < -40)
			{
				if($CA::Time <= 6)
					$CA::ClientCount--; // If a player dies within the first five seconds, exclude them from the "unstable" timer.
				else
				{
					$CA::ScoreBricks[%obj.bl_id] = $CA::ScoreBricks[%obj.bl_id]+%obj.player.bricksDestroyed;
					$CA::ScoreLoss[%obj.bl_id]++; // Counts as a loss if a) 
				}
				%obj.player.kill();
			}
			else if(isObject(%obj.player) && %lagCheck) // If player is alive and it has been at least one second since last check
			{
				if(%obj.player.getTransform() $= %obj.player.lastTransform && %obj.player.getVelocity() !$= "0 0 0")
				{
					talk("Client appears to be frozen: " @ %obj.name @ "; transform=" @ %obj.player.getTransform() @ "; last=" @ %obj.player.lastTransform);
					%obj.player.kill();
				}
				
				%obj.player.lastTransform = %obj.player.getTransform();
			}
			
			if(%obj.HUD)
				commandToClient(%obj,'bottomPrint',"<font:impact:45>\c3" @ $CA::TimeDisplay @ "<just:right>\c3" @ $CA::BrickCount-1 SPC "bricks left",0,1); // Exclude the music brick
		}
		
		$CA::Loop::Velocity = schedule(10,0,checkVelocity);
	}
};
activatePackage(CrumblingArenaPackage);
CALoop();
checkVelocity();