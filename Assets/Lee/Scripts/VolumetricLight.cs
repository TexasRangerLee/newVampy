using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System;

[RequireComponent(typeof(Light))]
public class VolumetricLight : MonoBehaviour 
{
    public event Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

    private Light _light;
    private Material _material;
    private CommandBuffer _commandBuffer;
    private CommandBuffer _cascadeShadowCommandBuffer; 

    public Light Light { get { return _light; } }
    public Material VolumetricMaterial { get { return _material; } }

    /// <summary>
    void Start() 
    {
        _commandBuffer = new CommandBuffer();
        _commandBuffer.name = "Light Command Buffer";

        _cascadeShadowCommandBuffer = new CommandBuffer();
        _cascadeShadowCommandBuffer.name = "Dir Light Command Buffer";
        _cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new UnityEngine.Rendering.RenderTargetIdentifier(UnityEngine.Rendering.BuiltinRenderTextureType.CurrentActive));

        _light = GetComponent<Light>();

        _light.AddCommandBuffer(LightEvent.AfterShadowMap, _commandBuffer);

        Shader shader = Shader.Find("VolumetricLight");
        if (shader == null)
            throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
        _material = new Material(shader); // new Material(VolumetricLightRenderer.GetLightMaterial());
    }

    /// <summary>
    void OnEnable()
    {
        VolumetricLightRenderer.PreRenderEvent += VolumetricLightRenderer_PreRenderEvent;
    }

    /// <summary>
    void OnDisable()
    {
        VolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
    }

    /// <summary>
    public void OnDestroy()
    {        
        Destroy(_material);
    }

    /// <summary>
    private void VolumetricLightRenderer_PreRenderEvent(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
    {
        // light was destroyed without deregistring, deregister now
        if (_light == null || _light.gameObject == null)
        {
            VolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
        }

        if (!_light.gameObject.activeInHierarchy || _light.enabled == false)
            return;

        _material.SetVector("_CameraForward", Camera.current.transform.forward);

		_material.SetInt("_SampleCount", 32);
        _material.SetVector("_NoiseVelocity", new Vector4(0.0f, 0.0f));
		_material.SetVector("_NoiseData", new Vector4(0.0f, 0.0f, 0.0f));
		_material.SetVector("_MieG", new Vector4(1.0f, 1.0f, 0.0f, 1.0f / (4.0f * Mathf.PI)));
		_material.SetVector("_VolumetricLight", new Vector4(1.0f, 0.0f, _light.range, 1.0f));
        
		_material.SetTexture("_CameraDepthTexture", renderer._halfVolumeLightTexture);
        
        if(_light.type == LightType.Point)
            SetupPointLight(renderer, viewProj);
        else if(_light.type == LightType.Spot)
            SetupSpotLight(renderer, viewProj);
    }

    void Update()
    {
        _commandBuffer.Clear();
    }

    /// <summary>
    private void SetupPointLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
    {
        int pass = 0;
        if (!IsCameraInPointLightBounds())
            pass = 2;

        _material.SetPass(pass);

		Mesh mesh = VolumetricLightRenderer._pointLightMesh;
        
        float scale = _light.range * 2.0f;
        Matrix4x4 world = Matrix4x4.TRS(transform.position, _light.transform.rotation, new Vector3(scale, scale, scale));

        _material.SetMatrix("_WorldViewProj", viewProj * world);
        _material.SetMatrix("_WorldView", Camera.current.worldToCameraMatrix * world);

        _material.DisableKeyword("NOISE");

        _material.SetVector("_LightPos", new Vector4(_light.transform.position.x, _light.transform.position.y, _light.transform.position.z, 1.0f / (_light.range * _light.range)));
        _material.SetColor("_LightColor", _light.color * _light.intensity);

        if (_light.cookie == null)
        {
            _material.EnableKeyword("POINT");
            _material.DisableKeyword("POINT_COOKIE");
        }
        else
        {
            Matrix4x4 view = Matrix4x4.TRS(_light.transform.position, _light.transform.rotation, Vector3.one).inverse;
            _material.SetMatrix("_MyLightMatrix0", view);

            _material.EnableKeyword("POINT_COOKIE");
            _material.DisableKeyword("POINT");
            
            _material.SetTexture("_LightTexture0", _light.cookie);
        }

        bool forceShadowsOff = false;
        if ((_light.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
            forceShadowsOff = true;

        if (_light.shadows != LightShadows.None && forceShadowsOff == false)
        {
            _material.EnableKeyword("SHADOWS_CUBE");
            _commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
			_commandBuffer.SetRenderTarget(renderer._halfVolumeLightTexture);

            _commandBuffer.DrawMesh(mesh, world, _material, 0, pass);

            if (CustomRenderEvent != null)
                CustomRenderEvent(renderer, this, _commandBuffer, viewProj);            
        }
        else
        {
            _material.DisableKeyword("SHADOWS_CUBE");
            renderer.GlobalCommandBuffer.DrawMesh(mesh, world, _material, 0, pass);
            
            if (CustomRenderEvent != null)
                CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
        }
    }

    /// <summary>
    private void SetupSpotLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
    {        
        int pass = 1;
        if (!IsCameraInSpotLightBounds())
        {
            pass = 3;     
        }

		Mesh mesh = VolumetricLightRenderer._spotLightMesh;
                
        float scale = _light.range;
        float angleScale = Mathf.Tan((_light.spotAngle + 1) * 0.5f * Mathf.Deg2Rad) * _light.range;

        Matrix4x4 world = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(angleScale, angleScale, scale));

        Matrix4x4 view = Matrix4x4.TRS(_light.transform.position, _light.transform.rotation, Vector3.one).inverse;

        Matrix4x4 clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1.0f));
        Matrix4x4 proj = Matrix4x4.Perspective(_light.spotAngle, 1, 0, 1);

        _material.SetMatrix("_MyLightMatrix0", clip * proj * view);

        _material.SetMatrix("_WorldViewProj", viewProj * world);

        _material.SetVector("_LightPos", new Vector4(_light.transform.position.x, _light.transform.position.y, _light.transform.position.z, 1.0f / (_light.range * _light.range)));
        _material.SetVector("_LightColor", _light.color * _light.intensity);


        Vector3 apex = transform.position;
        Vector3 axis = transform.forward;
        // plane equation ax + by + cz + d = 0; precompute d here to lighten the shader
        Vector3 center = apex + axis * _light.range;
        float d = -Vector3.Dot(center, axis);

        // update material
        _material.SetFloat("_PlaneD", d);        
        _material.SetFloat("_CosAngle", Mathf.Cos((_light.spotAngle + 1) * 0.5f * Mathf.Deg2Rad));

        _material.SetVector("_ConeApex", new Vector4(apex.x, apex.y, apex.z));
        _material.SetVector("_ConeAxis", new Vector4(axis.x, axis.y, axis.z));

        _material.EnableKeyword("SPOT");

        _material.DisableKeyword("NOISE");


        bool forceShadowsOff = false;
        if ((_light.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
            forceShadowsOff = true;

        if (_light.shadows != LightShadows.None && forceShadowsOff == false)
        {
            clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));

            proj = Matrix4x4.Perspective(_light.spotAngle, 1, _light.range, _light.shadowNearPlane);

            Matrix4x4 m = clip * proj;
            m[0, 2] *= -1;
            m[1, 2] *= -1;
            m[2, 2] *= -1;
            m[3, 2] *= -1;

            //view = _light.transform.worldToLocalMatrix;
            _material.SetMatrix("_MyWorld2Shadow", m * view);
            _material.SetMatrix("_WorldView", m * view);

            _material.EnableKeyword("SHADOWS_DEPTH");
            _commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
			_commandBuffer.SetRenderTarget(renderer._halfVolumeLightTexture);

            _commandBuffer.DrawMesh(mesh, world, _material, 0, pass);

            if (CustomRenderEvent != null)
                CustomRenderEvent(renderer, this, _commandBuffer, viewProj);       
        }
        else
        {
            _material.DisableKeyword("SHADOWS_DEPTH");
            renderer.GlobalCommandBuffer.DrawMesh(mesh, world, _material, 0, pass);

            if (CustomRenderEvent != null)
                CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
        }
    }
		

    /// <summary>
    private bool IsCameraInPointLightBounds()
    {
        float distanceSqr = (_light.transform.position - Camera.current.transform.position).sqrMagnitude;
        float extendedRange = _light.range + 1;
        if (distanceSqr < (extendedRange * extendedRange))
            return true;
        return false;
    }

    /// <summary>
    private bool IsCameraInSpotLightBounds()
    {
        // check range
        float distance = Vector3.Dot(_light.transform.forward, (Camera.current.transform.position - _light.transform.position));
        float extendedRange = _light.range + 1;
        if (distance > (extendedRange))
            return false;

        // check angle
        float cosAngle = Vector3.Dot(transform.forward, (Camera.current.transform.position - _light.transform.position).normalized);
        if((Mathf.Acos(cosAngle) * Mathf.Rad2Deg) > (_light.spotAngle + 3) * 0.5f)
            return false;

        return true;
    }
}
