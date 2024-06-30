shader_type canvas_item;

uniform sampler2D bg : hint_screen_texture;

void fragment() {
	vec4 text = textureLod(bg, SCREEN_UV, 2.0);
	COLOR = text;	// Called for every pixel the material is visible on.
}