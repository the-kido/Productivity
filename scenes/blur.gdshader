shader_type canvas_item;

global uniform sampler2D bg;

uniform sampler2D screen : hint_screen_texture;

vec4 texture_xorgaussian(sampler2D tex, vec2 uv, vec2 pixel_size, float blurriness, float radius){
	float pi = 3.14;
	float n = 0.0015;
	
	vec4 blurred_tex = vec4(0);
	
	float weight;
	for (float i = -blurriness; i <= blurriness; i++){
		float d = i / pi;
		vec2 anchor = vec2(cos(d), sin(d)) * radius * i;
		vec2 directions = uv + pixel_size * anchor;
		blurred_tex += texture(tex, directions) * n;
		if (i <= 0.0) {n += 0.0015; }
		if (i > 0.0) {n -= 0.0015; }
		weight += n;
	}
	
	float norm = 1.0 / weight;
	blurred_tex *= norm;
	return blurred_tex;
}
void fragment() {
	//vec4 text = textureLod(bg, SCREEN_UV, 10.0);
	//text += texture(img, UV);
	
	COLOR = texture_xorgaussian(bg, SCREEN_UV, SCREEN_PIXEL_SIZE, 8.0, 2);
	//vec4 text = textureLod(img, UV, 10.0);
	//COLOR = text;	// Called for every pixel the material is visible on.
}
