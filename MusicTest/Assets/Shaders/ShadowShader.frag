#version v

in vec4 vertColor;
in vec2 UV;
out vec4 fragColor;

void main()
{
  fragColor = vec4(vertColor.rgb, 0.4);
  if(fragColor.a < 0.01f) discard;
}