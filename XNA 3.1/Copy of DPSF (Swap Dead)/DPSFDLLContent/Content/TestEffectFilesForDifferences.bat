@ECHO OFF

REM If the DPSFDefaultEffect.fx file does not equal the Xbox360.fx file display that the files are different
fc "DPSFDefaultEffect.fx" "DPSFDefaultEffectXbox360.fx"
if errorlevel 1 echo The files are different

PAUSE