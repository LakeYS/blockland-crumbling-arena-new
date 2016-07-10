//Made by Chrono, edited by Mr.Noﬂody

if(isObject(MissionMusic))
	return;

brickMusicData.musicRange = 30;
brickMusicData.musicDescription = AudioMusicLooping3d;

package customrangemusic
{
	function fxDtsBrick::setSound(%this,%music,%client)
	{
		if(isObject(%this.audioEmitter))
			%this.audioEmitter.delete();
		if(!%music || %music == -1)
			return 1;
		%this.audioEmitter = new AudioEmitter()
		{
			position = %this.position;
			rotation = "1 0 0 0";
			scale = "1 1 1";
			profile = %music;
			useProfileDescription = "0";
			description = %this.dataBlock.musicDescription;
			type = "0";
			volume = "1";
			outsideAmbient = "1";
			referenceDistance = "1";
			maxDistance = %this.dataBlock.musicRange;
			isLooping = "1";
			loopCount = "-1";
			minLoopGap = "0";
			maxLoopGap = "0";
			enableVisualFeedback = "1";
			is3D = "1";
			coneInsideAngle = "360";
			coneOutsideAngle = "360";
			coneOutsideVolume = "1";
			coneVector = "0 1 0";
		};
	}
};
activatePackage(customrangemusic);