using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;
    public float jumpForce = 5;
    PlayerController controller;
    Camera viewCamera;
    private Plane plane = new Plane(Vector3.up, new Vector3(0, 1.3f, 0));
    GunController gunController;
    Rigidbody body;
    bool shootMode;
    public CrossHairs crossHairs;
    public bool isAiming = false;
    public float aimingTime;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        body = GetComponent<Rigidbody>();
        shootMode = gunController.equipedGun.shootMode;
    }

    void Update()
    {
        // 移动输入
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //瞄准输入
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crossHairs.transform.position = point;
            crossHairs.DetectTarget(ray);
        }

        //开枪输入
        if (shootMode) { //连发
            if (Input.GetMouseButton(0)){
                gunController.Shoot();
            }
        }
        else {  //单发
            if (Input.GetMouseButtonDown(0)){
                gunController.Shoot();
                Time.timeScale = 1f;
                isAiming = false;
            }
        }

        if (!shootMode) {
            if (Input.GetMouseButtonDown(1)) {
                Time.timeScale = 0.5f;
                isAiming = true;
                aimingTime = Time.time;
            }
            if (Input.GetMouseButtonUp(1)) {
                Time.timeScale = 1f;
                isAiming = false;
            }
        }
        else {
            Time.timeScale = 1f;
            isAiming = false;
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) {
            gunController.EquipLastGun();
            shootMode = gunController.equipedGun.shootMode;
        }
        else if (scroll < 0) {
            gunController.EquipNextGun();
            shootMode = gunController.equipedGun.shootMode;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            gunController.equipedGun.Reload();
        }

        //跳跃
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 1.05f) {
            body.AddForce(Vector3.up * jumpForce);
        }
    }
}
