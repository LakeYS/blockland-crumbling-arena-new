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