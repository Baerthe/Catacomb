/*
Shader from Godot Shaders - the free shader library.
This shader is under CC0 license. -> godotshaders.com.

Bloom, border and distortion changes, refactorting was by Baerthe
Original Optimised and packed was by @c64cosmin
CRT grille and rolling lines made by @c64cosmin
Vignette and warping effect was made by pend00
Scanlines are from "TimothyLottes" FROM SHADERTOY
Then ported by AHOPNESS (@ahopness)
*/
shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D reflection_texture : hint_default_black;
uniform sampler2D smudge_texture : hint_default_black, filter_linear_mipmap;

uniform vec2 resolution = vec2(1240.0, 960.0);

uniform float warp_amount :hint_range(0.0, 5.0) = 0.1;
uniform float noise_amount :hint_range(0.0, 0.3) = 0.03;
uniform float interference_amount :hint_range(0.0, 1.0) = 0.2;

uniform float grille_amount :hint_range(0.0, 1.0) = 0.1;
uniform float grille_size :hint_range(1.0, 5.0) = 1.0;

uniform float vignette_amount :hint_range(0.0, 2.0) = 0.6;
uniform float vignette_intensity : hint_range(0.0, 1.0) = 0.4;
uniform float aberation_amount :hint_range(0.0, 1.0) = 0.25;

uniform float roll_line_amount :hint_range(0.0, 1.0) = 0.3;
uniform float roll_speed :hint_range(-8.0, 8.0) = 1.0;

uniform float scan_line_amount :hint_range(0.0, 0.5) = 0.05;
uniform float scan_line_strength :hint_range(-12.0, -1.0) = -8.0;

uniform float pixel_strength :hint_range(-4.0, 0.0) = -2.0;
uniform float bloom_threshold :hint_range(0.0, 1.0) = 0.124;
uniform float bloom_multiplier :hint_range(0.0, 5.0) = 0.45;
uniform float bloom_radius :hint_range(0, 5.0) = 1.2;

uniform float border_grille_amount :hint_range(0.0, 0.2) = 0.03;
uniform float border_feather :hint_range(0.001, 0.04) = 0.004;
uniform float border_black_level :hint_range(0.0, 0.8) = 0.0;
uniform float border_width :hint_range(0.0, 0.08) = 0.02;
uniform float border_corner_radius :hint_range(0.0, 0.2) = 0.035;

uniform float reflection_strength :hint_range(0.0, 1.0) = 0.12;
uniform float reflection_curve_boost :hint_range(0.0, 2.0) = 0.9;
uniform vec2 reflection_scale = vec2(1.1, 1.1);
uniform vec2 reflection_drift = vec2(0.0, 0.0);
uniform float smudge_amount :hint_range(0.0, 1.0) = 0.5;

float random(vec2 uv){
    return fract(cos(uv.x * 83.4827 + uv.y * 92.2842) * 43758.5453123);
}

float bounds_dist(vec2 p){
	vec2 box_d = abs(p - 0.5) - (0.5 - border_width - border_corner_radius);
	return length(max(box_d, 0.0)) + min(max(box_d.x, box_d.y), 0.0) - border_corner_radius;
}

vec3 fetch_pixel(vec2 uv, vec2 off, vec2 screen_uv_origin, vec2 ratio){
	vec2 pos = floor(uv * resolution + off) / resolution + vec2(0.5) / resolution;
	float noise = 0.0;
	if(noise_amount > 0.0){
		noise = random(pos + fract(TIME)) * noise_amount;
	}
	if(bounds_dist(pos) > 0.0){
		return vec3(0.0, 0.0, 0.0);
	}
	vec2 screen_sample_uv = screen_uv_origin + pos * ratio;
	vec3 clr = texture(SCREEN_TEXTURE, screen_sample_uv).rgb + noise;
	return clr;
}

vec2 Dist(vec2 pos){
	pos = pos * resolution;
	return - ((pos - floor(pos)) - vec2(0.5));
}

float Gaus(float pos, float scale){ return exp2(scale * pos * pos); }

vec3 Horz3(vec2 pos, float off, vec2 screen_uv_origin, vec2 ratio){
	vec3 b = fetch_pixel(pos, vec2(-1.0, off), screen_uv_origin, ratio);
	vec3 c = fetch_pixel(pos, vec2( 0.0, off), screen_uv_origin, ratio);
	vec3 d = fetch_pixel(pos, vec2( 1.0, off), screen_uv_origin, ratio);
	float dst = Dist(pos).x;
	float scale = pixel_strength;
	float wb = Gaus(dst - 1.0, scale);
	float wc = Gaus(dst + 0.0, scale);
	float wd = Gaus(dst + 1.0, scale);
	return (b * wb + c * wc + d * wd) / (wb + wc + wd);
}

float Scan(vec2 pos, float off){
	float dst = Dist(pos).y;
	return Gaus(dst + off, scan_line_strength);
}

vec3 Tri(vec2 pos, vec2 screen_uv_origin, vec2 ratio){
	vec3 clr = fetch_pixel(pos, vec2(0.0), screen_uv_origin, ratio);
	return clr;
}

vec2 warp(vec2 uv){
	vec2 delta = uv - 0.5;
	float delta2 = dot(delta.xy, delta.xy);
	float delta4 = delta2 * delta2;
	float delta_offset = delta4 * warp_amount;
	vec2 warped = uv + delta * delta_offset;
	return warped;
}

float vignette(vec2 uv){
	uv *= 1.0 - uv.xy;
	float vignette = uv.x * uv.y * 15.0;
	return pow(max(vignette, 0.0), vignette_intensity * vignette_amount);
}

vec3 grille(vec2 uv){
	float unit = PI / 3.0;
	float scale = 2.0*unit/grille_size;
	float r = smoothstep(0.5, 0.8, cos(uv.x*scale - unit));
	float g = smoothstep(0.5, 0.8, cos(uv.x*scale + unit));
	float b = smoothstep(0.5, 0.8, cos(uv.x*scale + 3.0*unit));
	return mix(vec3(1.0), vec3(r,g,b), grille_amount);
}

float border_grille_luma(vec2 uv){
	float unit = PI / 3.0;
	float scale = 2.0*unit/grille_size;
	float r = smoothstep(0.5, 0.8, cos(uv.x*scale - unit));
	float g = smoothstep(0.5, 0.8, cos(uv.x*scale + unit));
	float b = smoothstep(0.5, 0.8, cos(uv.x*scale + 3.0*unit));
	return dot(vec3(r, g, b), vec3(0.33333334));
}

float roll_line(vec2 uv){
	float x = uv.y * 3.0 - TIME * roll_speed;
	float f = cos(x) * cos(x * 2.35 + 1.1) * cos(x * 4.45 + 2.3);
	float roll_line = smoothstep(0.5, 0.9, f);
	float ray_width_cap = smoothstep(0.02, 0.08, uv.x) * smoothstep(0.98, 0.92, uv.x);
	return roll_line * roll_line_amount * ray_width_cap;
}

vec3 reflection_layer(vec2 uv, float edge_factor){
	vec2 delta = uv - 0.5;
	float delta2 = dot(delta.xy, delta.xy);
	float delta4 = delta2 * delta2;
	vec2 refl_warp = uv + delta * (delta4 * warp_amount * 2.0);
	vec2 refl_uv = (refl_warp - 0.5) * reflection_scale + 0.5;
	vec3 refl = texture(reflection_texture, refl_uv).rgb;
	float edge_gloss = mix(0.25, 1.0, pow(edge_factor, 1.5) * reflection_curve_boost);
	return refl * edge_gloss;
}

void fragment(){
	float rx = abs(dFdx(UV.x)) > abs(dFdy(UV.x)) ? dFdx(SCREEN_UV.x) / dFdx(UV.x) : dFdy(SCREEN_UV.x) / dFdy(UV.x);
	float ry = abs(dFdy(UV.y)) > abs(dFdx(UV.y)) ? dFdy(SCREEN_UV.y) / dFdy(UV.y) : dFdx(SCREEN_UV.y) / dFdx(UV.y);
	vec2 ratio = vec2(rx, ry);
	vec2 screen_uv_origin = SCREEN_UV - UV * ratio;

	vec2 pix = UV * resolution;
	vec2 physical_pos = warp(UV);
	float line = 0.0;
	if(roll_line_amount > 0.0){
		line = roll_line(physical_pos);
	}
	vec2 sq_pix = floor(physical_pos * resolution) / resolution + vec2(0.5) / resolution;
	vec2 signal_pos = physical_pos;
	if(interference_amount + roll_line_amount > 0.0){
		float interference = random(sq_pix.yy + fract(TIME));
		signal_pos.x += (interference * (interference_amount + line * 6.0)) / resolution.x;
	}
	vec3 clr = Tri(signal_pos, screen_uv_origin, ratio);
	if(aberation_amount > 0.0){
		float center_dist = clamp(length((signal_pos - 0.5) * 2.0), 0.0, 1.0);
		float chromatic = aberation_amount * center_dist * center_dist;
		vec2 chromatic_x = vec2(chromatic,0.0) / resolution.x;
		float r = Tri(signal_pos - chromatic_x, screen_uv_origin, ratio).r;
		float g = Tri(signal_pos, screen_uv_origin, ratio).g;
		float b = Tri(signal_pos + chromatic_x, screen_uv_origin, ratio).b;
		clr = vec3(r,g,b);
	}
	float box_dist = bounds_dist(physical_pos);
	float is_inside_warp = 1.0 - smoothstep(0.0 - border_feather, 0.0, box_dist);
	if(grille_amount > 0.0)clr *= grille(pix);
	if(scan_line_amount > 0.0){
		float scan_y = mix(UV.y, physical_pos.y, 0.25);
		float scan_freq = floor(resolution.y * 2.0);
		float scan = 0.5 + 0.5 * cos(scan_y * PI * 2.0 * scan_freq);
		scan = pow(scan, abs(scan_line_strength) * 0.3);
		clr *= mix(1.0, scan, scan_line_amount);
	}
	clr *= 1.0 + scan_line_amount * 0.6 + line * 3.0 + grille_amount * 2.0;
	if(vignette_amount > 0.0)clr *= vignette(physical_pos);
	if(bloom_multiplier > 0.0){
		vec3 bloom = vec3(0.0);
		vec2 blur_size = vec2(abs(bloom_radius)) / resolution;
		for(float x = -2.0; x <= 2.0; x += 1.0){
			for(float y = -2.0; y <= 2.0; y += 1.0){
				vec2 offset = vec2(x, y) * blur_size;
				vec2 sample_pos = signal_pos + offset;
				float sample_mask = step(bounds_dist(physical_pos + offset), 0.0);
				vec2 sample_screen_uv = screen_uv_origin + sample_pos * ratio;
				vec3 sample_clr = texture(SCREEN_TEXTURE, sample_screen_uv).rgb * sample_mask;
				bloom += max(vec3(0.0), sample_clr - bloom_threshold);
			}
		}
		bloom /= 25.0;
		clr += bloom * bloom_multiplier;
	}
	if(reflection_strength > 0.0){
		float edge_factor = clamp(length((physical_pos - 0.5) * 2.0), 0.0, 1.0);
		clr += reflection_layer(UV, edge_factor) * reflection_strength;
	}
	if(smudge_amount > 0.0){
		float smudges = texture(smudge_texture, UV).r;
		clr *= 1.0 - (smudges * smudge_amount * 0.5);
		clr += smudges * smudge_amount * reflection_strength * 0.8;
	}
	vec3 screen_clr = clamp(clr, 0.0, 1.0) * is_inside_warp;
	float border_region = 1.0 - is_inside_warp;
	vec3 bezel_clr = vec3(border_black_level + border_grille_luma(pix) * border_grille_amount) * border_region;
	float border = smoothstep(0.0, 0.01, UV.x) * smoothstep(1.0, 0.99, UV.x) * smoothstep(0.0, 0.01, UV.y) * smoothstep(1.0, 0.99, UV.y);
	clr = (screen_clr + bezel_clr) * border;
	COLOR.rgb = clr;
	COLOR.a = 1.0;
}