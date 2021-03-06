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
			return "brick2x4x3Data";

		case 6:
			return "brick2x2Data";

		case 7:
			return "brick2xCubeData";
	}
}

function getBrickSize(%brick)
{
	return %brick.brickSizeX/2 SPC %brick.brickSizeY/2 SPC %brick.brickSizeZ/5;
}

function buildArena()
{
	%arenaSizeX = 15 + ClientGroup.getCount();
	%arenaSizeY = 15 + ClientGroup.getCount();

	if(%arenaSizeX > 24)
	{
		%arenaSizeX = 24;
		%arenaSizeY = 24;
	}

	%arenaHeight = getRandom(2,9);
	$CA::Layers = %arenaHeight;

	$CA::BrickDatablock = getBrick(getRandom(0,7));
	%brickSize = getBrickSize($CA::BrickDatablock);

	// This adjusts the arena size for different brick sizes.
	%arenaSizeX *= $CA::BrickDatablock.brickSizeY/$CA::BrickDatablock.brickSizeX;

	// May help with the glitched bricks
	%arenaOffsetX = getRandom(-700,700);
	%arenaOffsetY = getRandom(-700,700);

	// Pick a random shape
	//%arenaShape = getRandom(0,5);
	%arenaShape = 6; //TEMPORARY

	switch(%arenaShape)
	{
		case 2: // Arena shape 3: rectangle.
			if(getRandom(1,2))
				%arenaSizeX /= 2;
			else
				%arenaSizeY /= 2;
	}

	echo("SIZE=" @ %arenaSizeX SPC "HEIGHT=" @ %arenaHeight SPC "SHAPE=" @ %arenaShape SPC "BRICK=" @ $CA::BrickDatablock);

	%timeStart = getRealTime();

	//talk(%arenaSizeX*getWord(%brickSize,1)*2);

	$CA::ArenaSizeDisplay = %arenaSizeX @ "x" @ %arenaSizeY @ "x" @ %arenaHeight @ %f;
	$CA::ArenaBrick = $CA::BrickDatablock.uiName;
	$CA::ArenaIcon = $CA::BrickDatablock.iconName;

	for(%z=0;%z<%arenaHeight;%z++)
	{
		%positionZ = 200 + getWord(%brickSize,2)*%z;

		for(%x=0;%x<%arenaSizeX;%x++)
		{
			%positionX = %arenaOffsetX + (getWord(%brickSize,0)*%x);

			for(%y=0;%y<%arenaSizeY;%y++)
			{
				%skipBrick = 0;

				//To do:
				// Rewrite spawning system for shaped arenas
				// Consider removal/improvement of Z shape
				// Rectangle arenas
				// Long arenas
				// Circle arenas?
				// Correct arena size for smaller bricks

				switch(%arenaShape)
				{
					case 0: // Arena shape 1; Z shape. This will require some changes to the spawn function so players can spawn on either side without dying.
						if(%y >= mCeil(%arenaSizeY/2) && %x >= mCeil(%arenaSizeX/2) || %y <= mCeil((%arenaSizeY/2)-1) && %x <= mCeil((%arenaSizeX/2)-1))
							%skipBrick = 1;
					case 1: // Arena shape 2: plus shape A
						if(%x >= %arenaSizeX-5 && %y >= %arenaSizeY-5 || %x <= 4 && %y <= 4 || %x >= %arenaSizeX-5 && %y <= 4 || %x <= 4 && %y >= %arenaSizeY-5)
							%skipBrick = 1;
				}

				if(!%skipBrick)
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

					//if(%y >= mCeil(%arenaSizeX/2) && %x >= mCeil(%arenaSizeX/2) || %y <= mCeil((%arenaSizeX/2)-1) && %x <= mCeil((%arenaSizeX/2)-1))
					//	%brick.setColor(%z+1);

					//// Uncomment to re-color edges of the arena
					//if(%z == %arenaHeight-1)
					//{
					//	if(%y == %arenaSizeX-1 || %y == 0 || %x == %arenaSizeX-1 || %x == 0)
					//		%brick.setColor(%arenaHeight-2);
					//}

					// This is for testing purposes.
					%brick.y = %y; %brick.x = %x; %brick.z = %z;

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

	%timeEnd = getRealTime();

	%timeElapsed = %timeEnd-%timeStart;

	echo("Arena built in" SPC %timeElapsed @ "ms");

	if(clientGroup.getCount() != 0) // Don't make the spawns if the server is empty -- we will wait until someone joins to do that.
		$CA::Loop::MakeSpawn = schedule(33,0,makeNewSpawn,%positionX,%positionY,%positionZ + 20,%firstBrick,%lastBrick);
	else
	{
		$CA::MS1 = %positionX;
		$CA::MS2 = %positionY;
		$CA::MS3 = %positionZ + 20;
		$CA::MS4 = %firstBrick;
		$CA::MS5 = %lastBrick;
	}
}
