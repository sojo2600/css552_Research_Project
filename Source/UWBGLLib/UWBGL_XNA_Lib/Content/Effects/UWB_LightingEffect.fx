//
//	UWB_LightingEffect
//
//  Single-pass shader for multiple lights in XNA based on the 
//  D3D9 lighting specifications. 
//	* Requires shader version 3_0

float4x4 world;
float4x4 view;
float4x4 projection;
float3 cameraPosition;

// Light types
shared int POINTLIGHT = 0;
shared int DIRECTIONLIGHT = 1;
shared int SPOTLIGHT = 2;

struct Light
{
	int type;				// 0=Point Light 1=Directional Light 2=Spotlight
	bool on;				// For switching light contribution on/off
	float4 ambient;
	float4 diffuse;
	float4 specular;
	float3 position;
	float3 direction;
	float range;
	float falloff;
	float3 attenuation;		// [0]=constant [1]=linear [2]=quadratic
	float theta;			// inner spotlight angle (umbra)
	float phi;				// outer spotlight angle (penumbra)
};

shared Light lights[8];
shared float numLights = 0;

struct Material
{
	float4 ambient;
	float4 diffuse;
	float4 specular;
	float4 emissive;
	float shininess;	// range 0 -> +infinity
};

shared Material material;

texture textureMap;
shared bool textureEnable;

sampler2D textureSampler = sampler_state
{
	Texture = <textureMap>;
    MagFilter = Linear;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MaxAnisotropy = 16;
};

struct VertexShaderInput
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;

};

struct VertexShaderOutput
{
     float4 Position : POSITION;
     float2 TexCoords : TEXCOORD0;
     float3 WorldNormal : TEXCOORD1;
     float3 WorldPosition : TEXCOORD2;
};

VertexShaderOutput LightingVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    // world view projection matrix
	float4x4 wvp = mul(mul(world, view), projection);
     
	// transform the input position to the output
	output.Position = mul(float4(input.Position, 1.0), wvp);

	output.WorldNormal =  mul(input.Normal, world);
	float4 worldPosition =  mul(float4(input.Position, 1.0), world);
	output.WorldPosition = worldPosition / worldPosition.w;
	
	output.TexCoords = input.TexCoord;

    return output;
}

float4 SingleLight(Light light, float3 worldPosition, float3 worldNormal)
{
    float3 lightVector = light.position - worldPosition;
    float distToLight = length(lightVector);
    float3 directionToLight = normalize(lightVector);
    	
    // Calculate attenuation based on MSDN D3D9 docs
    // at http://msdn.microsoft.com/en-us/library/bb172279(VS.85).aspx
    //
    float atten = 1.0f;
    
    if (light.type == DIRECTIONLIGHT)
    {
		atten = 1.0f;
    }
    else if (distToLight > light.range)
    {
		atten = 0.0f;
	}
    else
    {
		atten = 1.0f / (light.attenuation[0] 
			+ light.attenuation[1] * distToLight 
			+ light.attenuation[2] * distToLight * distToLight);
	}
	
	// Calculate Spotlight Factor based on D3D9 docs
	// at http://msdn.microsoft.com/en-us/library/bb172279(VS.85).aspx
	//     
	float halfPenumbra = cos(light.phi * 0.5f);
    float halfUmbra = cos(light.theta * 0.5f);
    
    // rho is the angle between the direction vector of the light
    // and the direction vector to the light from the vertex
    float rho = dot(-normalize(light.direction), directionToLight); 
    
    float spotFactor = 1.0f;
    
    if (light.type != SPOTLIGHT || rho > halfUmbra)
    {
		spotFactor = 1.0f;
	}
	else if (rho <= halfPenumbra || halfUmbra - halfPenumbra == 0.0f)
	{
		spotFactor = 0.0f;
	}
	else
	{	
		// Avoided dividing by zero above
		spotFactor = smoothstep(0, 1, 
			pow((rho - halfPenumbra)/(halfUmbra - halfPenumbra),
			light.falloff));
    }
    
    // Add the ambient contribution
    float4 color = material.ambient * light.ambient * atten * spotFactor;
   
	float3 N = normalize(worldNormal);   // Surface normal
	float3 halfWayVector = normalize(normalize(cameraPosition - worldPosition) 
		+ directionToLight);
	
	// Add the specular contribution
	color += material.specular * light.specular
		* pow(saturate(dot(N, halfWayVector)), material.shininess) 
		* atten * spotFactor;
    
    // Add the diffuse contribution
    color += material.diffuse 
		* light.diffuse * saturate(dot(N, directionToLight)) 
		* atten * spotFactor;
   
	return color;
}

float4 MultiLightPS(VertexShaderOutput input) : COLOR
{
	// Not using a global amient
	float4 color = {0.0f, 0.0f, 0.0f, 0.0f};
	
	if (textureEnable)
		color = tex2D(textureSampler, input.TexCoords);
	
    color += material.emissive;
    
    for(int i=0; i< numLights; i++)
    {
		if (lights[i].on == true)
		{
			color += SingleLight(lights[i], input.WorldPosition, input.WorldNormal);
		}
    }
    color.a = 1.0;
    return color;
}

technique MultipleLights
{
    pass P0
    {
        VertexShader = compile vs_3_0 LightingVS();
        PixelShader = compile ps_3_0 MultiLightPS();
    }
}

