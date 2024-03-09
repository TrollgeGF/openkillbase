using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class NewMovement : MonoBehaviour
{
	private InputManager inman;

	public float walkSpeed;

	public float jumpPower;

	public float airAcceleration;

	public float wallJumpPower;

	private bool jumpCooldown;

	private bool falling;

	public Rigidbody rb;

	private Vector3 movementDirection;

	private Vector3 movementDirection2;

	private Vector3 airDirection;

	public float timeBetweenSteps;

	private float stepTime;

	private int currentStep;

	public Animator anim;

	private GameObject body;

	private Quaternion tempRotation;

	private GameObject forwardPoint;

	public GroundCheck gc;

	public GroundCheck slopeCheck;

	private WallCheck wc;

	private PlayerAnimations pa;

	private Vector3 wallJumpPos;

	private int currentWallJumps;

	private AudioSource aud;

	private AudioSource aud2;

	private AudioSource aud3;

	private int currentSound;

	public AudioClip jumpSound;

	public AudioClip landingSound;

	public AudioClip finalWallJump;

	public bool walking;

	public int hp = 100;

	public Image hurtScreen;

	private AudioSource hurtAud;

	private Color hurtColor;

	private Color currentColor;

	private bool hurting;

	public bool dead;

	public Image blackScreen;

	private Color blackColor;

	public Text youDiedText;

	private Color youDiedColor;

	public Image greenHpFlash;

	private Color greenHpColor;

	private AudioSource greenHpAud;

	public AudioMixer audmix;

	private float currentAllPitch = 1f;

	private float currentAllVolume;

	public bool boost;

	public Vector3 dodgeDirection;

	private float boostLeft;

	public float boostCharge = 300f;

	public AudioClip dodgeSound;

	public CameraController cc;

	private AudioSource ccAud;

	public GameObject staminaFailSound;

	public GameObject screenHud;

	private Vector3 hudOriginalPos;

	public GameObject dodgeParticle;

	public GameObject scrnBlood;

	private Canvas fullHud;

	private GameObject hudCam;

	private Vector3 camOriginalPos;

	private RigidbodyConstraints defaultRBConstraints;

	private GameObject revolver;

	private StyleHUD shud;

	public GameObject scrapePrefab;

	private GameObject scrapeParticle;

	public LayerMask lmask;

	public StyleCalculator scalc;

	public bool activated = false;

	private float fallSpeed;

	public bool jumping;

	private float fallTime;

	public GameObject impactDust;

	public GameObject fallParticle;

	private GameObject currentFallParticle;

	private CapsuleCollider playerCollider;

	public bool sliding;

	private float slideSafety;

	public GameObject slideParticle;

	private GameObject currentSlideParticle;

	public GameObject slideScrapePrefab;

	private GameObject slideScrape;

	private Vector3 slideMovDirection;

	public GameObject slideStopSound;

	private GunControl gunc;

	public float currentSpeed;

	private Punch punch;

	public GameObject dashJumpSound;

	public bool slowMode;

	public Vector3 pushForce;

	private float slideLength;

	public float longestSlide;

	public bool quakeJump;

	public GameObject quakeJumpSound;

	private bool exploded;

	private float clingFade;

	public float recentHealth;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		rb = GetComponent<Rigidbody>();
		aud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		body = GameObject.FindWithTag("Body");
		wc = GetComponentInChildren<WallCheck>();
		aud2 = gc.GetComponent<AudioSource>();
		pa = GetComponentInChildren<PlayerAnimations>();
		aud3 = wc.GetComponent<AudioSource>();
		cc = GetComponentInChildren<CameraController>();
		ccAud = cc.GetComponent<AudioSource>();
		hurtColor = hurtScreen.color;
		currentColor = hurtColor;
		currentColor.a = 0f;
		hurtScreen.color = currentColor;
		hurtAud = hurtScreen.GetComponent<AudioSource>();
		blackColor = blackScreen.color;
		youDiedColor = youDiedText.color;
		greenHpColor = greenHpFlash.color;
		screenHud = GetComponentInChildren<Canvas>().transform.parent.gameObject;
		fullHud = hurtScreen.GetComponentInParent<Canvas>();
		hudCam = screenHud.GetComponentInParent<Camera>().gameObject;
		hudOriginalPos = screenHud.transform.localPosition;
		camOriginalPos = hudCam.transform.localPosition;
		currentAllPitch = 1f;
		audmix.SetFloat("allPitch", 1f);
		defaultRBConstraints = rb.constraints;
		playerCollider = GetComponent<CapsuleCollider>();
		scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
	}

	private void Update()
	{
		float num = 0f;
		float num2 = 0f;
		if (activated)
		{
			num2 = (Input.GetKey(inman.inputs["W"]) ? 1f : ((!Input.GetKey(inman.inputs["S"])) ? 0f : (-1f)));
			num = (Input.GetKey(inman.inputs["A"]) ? (-1f) : ((!Input.GetKey(inman.inputs["D"])) ? 0f : 1f));
			cc.movementHor = num;
			cc.movementVer = num2;
			movementDirection = (num * base.transform.right + num2 * base.transform.forward).normalized;
			if (punch == null)
			{
				punch = GetComponentInChildren<Punch>();
			}
			else if (!punch.enabled)
			{
				punch.enabled = true;
			}
		}
		else
		{
			rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
			if (currentFallParticle != null)
			{
				Object.Destroy(currentFallParticle);
			}
			if (currentSlideParticle != null)
			{
				Object.Destroy(currentSlideParticle);
			}
			else if (slideScrape != null)
			{
				Object.Destroy(slideScrape);
			}
			if (punch == null)
			{
				punch = GetComponentInChildren<Punch>();
			}
			else
			{
				punch.enabled = false;
			}
		}
		if (dead)
		{
			currentAllPitch -= 0.1f * Time.deltaTime;
			audmix.SetFloat("allPitch", currentAllPitch);
			if (blackColor.a < 0.5f)
			{
				blackColor.a += 0.75f * Time.deltaTime;
				youDiedColor.a += 0.75f * Time.deltaTime;
			}
			else
			{
				blackColor.a += 0.05f * Time.deltaTime;
				youDiedColor.a += 0.05f * Time.deltaTime;
			}
			blackScreen.color = blackColor;
			youDiedText.color = youDiedColor;
		}
		if (gc.onGround != pa.onGround)
		{
			pa.onGround = gc.onGround;
		}
		if (!gc.onGround)
		{
			if (!falling)
			{
				fallTime += Time.deltaTime * 5f;
				if (fallTime > 1f)
				{
					falling = true;
				}
			}
			else if (rb.velocity.y < -2f)
			{
				fallSpeed = rb.velocity.y;
			}
		}
		else if (gc.onGround)
		{
			fallTime = 0f;
			clingFade = 0f;
		}
		if (!gc.onGround && rb.velocity.y < -20f)
		{
			aud3.pitch = rb.velocity.y * -1f / 120f;
			if (activated)
			{
				aud3.volume = rb.velocity.y * -1f / 80f;
			}
			else
			{
				aud3.volume = rb.velocity.y * -1f / 240f;
			}
		}
		else if (rb.velocity.y > -20f)
		{
			aud3.pitch = 0f;
			aud3.volume = 0f;
		}
		if (rb.velocity.y < -100f)
		{
			rb.velocity = new Vector3(rb.velocity.x, -100f, rb.velocity.z);
		}
		if (!gc.onGround && falling && fallSpeed <= -50f)
		{
			gc.heavyFall = true;
		}
		if (gc.onGround && falling)
		{
			falling = false;
			if (fallSpeed > -50f)
			{
				aud2.clip = landingSound;
				aud2.volume = 0.5f + fallSpeed * -0.01f;
				aud2.Play();
			}
			else
			{
				GameObject gameObject = Object.Instantiate(impactDust, gc.transform.position, Quaternion.identity);
				gameObject.transform.forward = Vector3.up;
				cc.CameraShake(0.5f);
			}
			fallSpeed = 0f;
			gc.heavyFall = false;
			if (currentFallParticle != null)
			{
				Object.Destroy(currentFallParticle);
			}
		}
		if (!gc.onGround && activated && Input.GetKeyDown(inman.inputs["Slide"]))
		{
			if (playerCollider.height != 2.5f)
			{
				StopSlide();
			}
			if (boost)
			{
				boostLeft = 0f;
				boost = false;
			}
			RaycastHit hitInfo;
			if (fallTime > 0.5f && !Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out hitInfo, 3f, lmask))
			{
				if (boostCharge >= 100f)
				{
					boostCharge -= 100f;
					rb.velocity = new Vector3(0f, -100f, 0f);
					falling = true;
					fallSpeed = -100f;
					gc.heavyFall = true;
					if (currentFallParticle != null)
					{
						Object.Destroy(currentFallParticle);
					}
					currentFallParticle = Object.Instantiate(fallParticle, base.transform);
				}
				else
				{
					Object.Instantiate(staminaFailSound);
				}
			}
		}
		if (gc.heavyFall || sliding)
		{
			Physics.IgnoreLayerCollision(2, 12, true);
		}
		else
		{
			Physics.IgnoreLayerCollision(2, 12, false);
		}
		RaycastHit hitInfo2;
		if (!slopeCheck.onGround && !jumping && !boost && rb.velocity != Vector3.zero && Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out hitInfo2, 2f, lmask))
		{
			Vector3 target = new Vector3(base.transform.position.x, base.transform.position.y - hitInfo2.distance, base.transform.position.z);
			base.transform.position = Vector3.MoveTowards(base.transform.position, target, hitInfo2.distance * 0.1f);
			if (rb.velocity.y > 0f)
			{
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			}
		}
		RaycastHit hitInfo3;
		if (gc.heavyFall && Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out hitInfo3, 5f, lmask))
		{
			Breakable component = hitInfo3.collider.GetComponent<Breakable>();
			if (component != null && component.weak && !component.precisionOnly)
			{
				Object.Instantiate(impactDust, hitInfo3.point, Quaternion.identity);
				component.Break();
			}
		}
		if (activated)
		{
			if (Input.GetKeyDown(inman.inputs["Jump"]) && gc.onGround && !jumpCooldown)
			{
				Jump();
			}
			if (!gc.onGround && wc.onWall)
			{
				RaycastHit hitInfo4;
				if (Physics.Raycast(base.transform.position, movementDirection, out hitInfo4, 1f, lmask))
				{
					if (rb.velocity.y < -1f && !gc.heavyFall)
					{
						rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -1f, 1f), -2f * clingFade, Mathf.Clamp(rb.velocity.z, -1f, 1f));
						if (scrapeParticle == null)
						{
							scrapeParticle = Object.Instantiate(scrapePrefab, hitInfo4.point, Quaternion.identity);
						}
						scrapeParticle.transform.position = new Vector3(hitInfo4.point.x, hitInfo4.point.y + 1f, hitInfo4.point.z);
						scrapeParticle.transform.forward = hitInfo4.normal;
						clingFade = Mathf.MoveTowards(clingFade, 50f, Time.deltaTime * 4f);
					}
				}
				else if (scrapeParticle != null)
				{
					Object.Destroy(scrapeParticle);
					scrapeParticle = null;
				}
				if (Input.GetKeyDown(inman.inputs["Jump"]) && !jumpCooldown && currentWallJumps < 3)
				{
					WallJump();
				}
			}
			else if (scrapeParticle != null)
			{
				Object.Destroy(scrapeParticle);
				scrapeParticle = null;
			}
		}
		if (sliding)
		{
			slideLength += Time.deltaTime;
		}
		if (Input.GetKeyDown(inman.inputs["Slide"]) && gc.onGround && playerCollider.height == 2.5f)
		{
			StartSlide();
		}
		RaycastHit hitInfo5;
		if (Input.GetKeyDown(inman.inputs["Slide"]) && !gc.onGround && !sliding && !jumping && activated && Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out hitInfo5, 2f, lmask))
		{
			StartSlide();
		}
		if (Input.GetKeyUp(inman.inputs["Slide"]) && sliding)
		{
			StopSlide();
		}
		if (sliding)
		{
			if (currentSlideParticle != null)
			{
				currentSlideParticle.transform.position = base.transform.position + dodgeDirection * 10f;
			}
			if (slideSafety <= 0f)
			{
				Vector3 vector = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
				if (vector.magnitude < 10f)
				{
					StopSlide();
				}
			}
			else if (slideSafety > 0f)
			{
				slideSafety -= Time.deltaTime * 5f;
			}
			if (gc.onGround)
			{
				slideScrape.transform.position = base.transform.position + dodgeDirection * 2f;
				cc.CameraShake(0.1f);
			}
			else
			{
				slideScrape.transform.position = Vector3.one * 5000f;
			}
		}
		else if (currentSlideParticle != null)
		{
			Object.Destroy(currentSlideParticle);
		}
		else if (slideScrape != null)
		{
			Object.Destroy(slideScrape);
		}
		if (Input.GetKeyDown(inman.inputs["Dodge"]) && activated)
		{
			if (boostCharge >= 100f)
			{
				if (sliding)
				{
					StopSlide();
				}
				boostLeft = 100f;
				boost = true;
				dodgeDirection = movementDirection;
				if (dodgeDirection == Vector3.zero)
				{
					dodgeDirection = base.transform.forward;
				}
				Quaternion identity = Quaternion.identity;
				identity.SetLookRotation(dodgeDirection * -1f);
				Object.Instantiate(dodgeParticle, base.transform.position + dodgeDirection * 10f, identity);
				boostCharge -= 100f;
				if (dodgeDirection == base.transform.forward)
				{
					cc.dodgeDirection = 0;
				}
				else if (dodgeDirection == base.transform.forward * -1f)
				{
					cc.dodgeDirection = 1;
				}
				else
				{
					cc.dodgeDirection = 2;
				}
				aud.clip = dodgeSound;
				aud.volume = 1f;
				aud.pitch = 1f;
				aud.Play();
				if (gc.heavyFall)
				{
					fallSpeed = 0f;
					gc.heavyFall = false;
					if (currentFallParticle != null)
					{
						Object.Destroy(currentFallParticle);
					}
				}
			}
			else
			{
				Object.Instantiate(staminaFailSound);
			}
		}
		if (!walking && (num2 != 0f || num != 0f) && !sliding && gc.onGround)
		{
			walking = true;
			anim.SetBool("WalkF", true);
		}
		else if (walking && ((num2 == 0f && num == 0f) || !gc.onGround || sliding))
		{
			walking = false;
			anim.SetBool("WalkF", false);
		}
		if (hurting && hp > 0)
		{
			currentColor.a -= Time.deltaTime;
			hurtScreen.color = currentColor;
			if (currentColor.a <= 0f)
			{
				hurting = false;
			}
		}
		if (greenHpColor.a > 0f)
		{
			greenHpColor.a -= Time.deltaTime;
			greenHpFlash.color = greenHpColor;
		}
		if (boostCharge != 300f && !sliding && !slowMode)
		{
			if (boostCharge + 70f * Time.deltaTime < 300f)
			{
				boostCharge += 70f * Time.deltaTime;
			}
			else
			{
				boostCharge = 300f;
			}
		}
		Vector3 vector2 = hudOriginalPos - cc.transform.InverseTransformDirection(rb.velocity) / 1000f;
		float num3 = Vector3.Distance(vector2, screenHud.transform.localPosition);
		screenHud.transform.localPosition = Vector3.MoveTowards(screenHud.transform.localPosition, vector2, Time.deltaTime * 15f * num3);
		Vector3 vector3 = Vector3.ClampMagnitude(camOriginalPos - cc.transform.InverseTransformDirection(rb.velocity) / 350f * -1f, 0.2f);
		float num4 = Vector3.Distance(vector3, hudCam.transform.localPosition);
		hudCam.transform.localPosition = Vector3.MoveTowards(hudCam.transform.localPosition, vector3, Time.deltaTime * 25f * num4);
		if (recentHealth > 0f)
		{
			recentHealth = Mathf.MoveTowards(recentHealth, 0f, Time.deltaTime * (recentHealth / 10f));
		}
	}

	private void FixedUpdate()
	{
		if (!boost)
		{
			Move();
		}
		else
		{
			Dodge();
		}
	}

	private void Move()
	{
		if (!hurting)
		{
			base.gameObject.layer = 2;
			exploded = false;
		}
		if (gc.onGround && !jumping)
		{
			aud.pitch = 1f;
			currentWallJumps = 0;
			if (slowMode)
			{
				movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 1.25f, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime * 1.25f);
			}
			else
			{
				movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 2.5f, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime * 2.5f);
			}
			rb.velocity = movementDirection2 + pushForce;
			anim.SetBool("Run", false);
			return;
		}
		if (slowMode)
		{
			movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 1.25f, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime * 1.25f);
		}
		else
		{
			movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 2.5f, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime * 2.5f);
		}
		airDirection.y = 0f;
		if ((movementDirection2.x > 0f && rb.velocity.x < movementDirection2.x) || (movementDirection2.x < 0f && rb.velocity.x > movementDirection2.x))
		{
			airDirection.x = movementDirection2.x;
		}
		else
		{
			airDirection.x = 0f;
		}
		if ((movementDirection2.z > 0f && rb.velocity.z < movementDirection2.z) || (movementDirection2.z < 0f && rb.velocity.z > movementDirection2.z))
		{
			airDirection.z = movementDirection2.z;
		}
		else
		{
			airDirection.z = 0f;
		}
		rb.AddForce(airDirection.normalized * airAcceleration);
	}

	private void Dodge()
	{
		if (sliding)
		{
			rb.velocity = new Vector3(dodgeDirection.x * walkSpeed * Time.deltaTime * 4f, rb.velocity.y, dodgeDirection.z * walkSpeed * Time.deltaTime * 4f);
			return;
		}
		movementDirection2 = new Vector3(dodgeDirection.x * walkSpeed * Time.deltaTime * 2.5f, 0f, dodgeDirection.z * walkSpeed * Time.deltaTime * 2.5f);
		rb.velocity = movementDirection2 * 3f;
		base.gameObject.layer = 15;
		boostLeft -= 4f;
		if (boostLeft <= 0f)
		{
			boost = false;
			if (!gc.onGround)
			{
				rb.velocity = movementDirection2;
			}
		}
	}

	private void Jump()
	{
		jumping = true;
		Invoke("NotJumping", 0.25f);
		if (quakeJump)
		{
			Object.Instantiate(quakeJumpSound);
		}
		aud.clip = jumpSound;
		aud.volume = 0.75f;
		aud.pitch = 1f;
		aud.Play();
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		if (sliding)
		{
			if (slowMode)
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f);
			}
			else
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 2f);
			}
			StopSlide();
		}
		else if (boost)
		{
			if (boostCharge >= 100f)
			{
				boostCharge -= 100f;
				Object.Instantiate(dashJumpSound);
			}
			else
			{
				rb.velocity = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 2.5f, 0f, movementDirection.z * walkSpeed * Time.deltaTime * 2.5f);
				Object.Instantiate(staminaFailSound);
			}
			if (slowMode)
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 0.75f);
			}
			else
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 1.5f);
			}
		}
		else if (slowMode)
		{
			rb.AddForce(Vector3.up * jumpPower * 1500f * 1.25f);
		}
		else
		{
			rb.AddForce(Vector3.up * jumpPower * 1500f * 2.5f);
		}
		jumpCooldown = true;
		Invoke("JumpReady", 0.2f);
		boost = false;
	}

	private void WallJump()
	{
		jumping = true;
		Invoke("NotJumping", 0.25f);
		currentWallJumps++;
		if (quakeJump)
		{
			GameObject gameObject = Object.Instantiate(quakeJumpSound);
			gameObject.GetComponent<AudioSource>().pitch = 1.1f + (float)currentWallJumps * 0.05f;
		}
		aud.clip = jumpSound;
		aud.pitch += 0.25f;
		aud.volume = 0.75f;
		aud.Play();
		if (currentWallJumps == 3)
		{
			aud2.clip = finalWallJump;
			aud2.volume = 0.75f;
			aud2.Play();
		}
		wallJumpPos = base.transform.position - wc.GetClosestPoint();
		rb.velocity = new Vector3(0f, 0f, 0f);
		Vector3 vector = new Vector3(wallJumpPos.normalized.x, 0.75f, wallJumpPos.normalized.z);
		rb.AddForce(vector * wallJumpPower * 2000f);
		jumpCooldown = true;
		Invoke("JumpReady", 0.1f);
	}

	public void Launch(Vector3 position, float strength, float maxDistance = 1f)
	{
		bool flag = false;
		if (jumping)
		{
			flag = true;
		}
		jumping = true;
		Invoke("NotJumping", 0.5f);
		jumpCooldown = true;
		Invoke("JumpReady", 0.2f);
		boost = false;
		rb.velocity = Vector3.zero;
		Vector3 normalized = (base.transform.position - position).normalized;
		Vector3 vector = ((!flag) ? new Vector3(normalized.x * (maxDistance - Vector3.Distance(base.transform.position, position)) * strength * 1000f, strength * 500f * (maxDistance - Vector3.Distance(base.transform.position, position)), normalized.z * (maxDistance - Vector3.Distance(base.transform.position, position)) * strength * 1000f) : new Vector3(normalized.x * maxDistance * strength * 1000f, strength * 500f * maxDistance, normalized.z * maxDistance * strength * 1000f));
		rb.AddForce(Vector3.ClampMagnitude(vector, 1000000f));
	}

	private void JumpReady()
	{
		jumpCooldown = false;
	}

	public void GetHurt(int damage, bool invincible, float scoreLossMultiplier = 1f, bool explosion = false)
	{
		if (dead || (invincible && base.gameObject.layer == 15))
		{
			return;
		}
		if (explosion)
		{
			exploded = true;
		}
		if (invincible)
		{
			base.gameObject.layer = 15;
		}
		if (damage >= 50)
		{
			currentColor.a = 0.8f;
		}
		else
		{
			currentColor.a = 0.5f;
		}
		hurting = true;
		cc.CameraShake(damage / 20);
		hurtAud.pitch = Random.Range(0.8f, 1f);
		hurtAud.PlayOneShot(hurtAud.clip);
		if (hp - damage > 0)
		{
			hp -= damage;
		}
		else
		{
			hp = 0;
		}
		if (shud == null)
		{
			shud = GetComponentInChildren<StyleHUD>();
		}
		if (scoreLossMultiplier > 0.5f)
		{
			shud.RemovePoints(0);
			shud.DescendRank();
			if (damage >= 20 && shud.currentRank >= 4 && scoreLossMultiplier >= 1f)
			{
				shud.DescendRank();
			}
		}
		else
		{
			shud.RemovePoints(Mathf.RoundToInt(damage));
		}
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		component.stylePoints -= Mathf.RoundToInt((float)(damage * 5) * scoreLossMultiplier);
		component.tookDamage = true;
		if (hp == 0)
		{
			blackScreen.gameObject.SetActive(true);
			rb.constraints = RigidbodyConstraints.None;
			ccAud.Play();
			cc.enabled = false;
			if (gunc == null)
			{
				gunc = GetComponentInChildren<GunControl>();
			}
			gunc.NoWeapon();
			rb.constraints = RigidbodyConstraints.None;
			dead = true;
			activated = false;
			screenHud.SetActive(false);
			if (punch == null)
			{
				punch = GetComponentInChildren<Punch>();
			}
			punch.enabled = false;
			punch.transform.GetChild(0).gameObject.SetActive(false);
			punch.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		}
	}

	public void GetHealth(int health, bool silent)
	{
		if (dead || exploded)
		{
			return;
		}
		float num = health;
		if (recentHealth > 0f)
		{
			num = (float)health / (recentHealth / 15f);
		}
		if (num >= 1f)
		{
			if ((float)hp + num < 100f)
			{
				recentHealth += Mathf.RoundToInt(num);
				hp += Mathf.RoundToInt(num);
			}
			else if (hp != 100)
			{
				recentHealth += 100 - hp;
				hp = 100;
			}
		}
		greenHpColor.a = 1f;
		greenHpFlash.color = greenHpColor;
		if (!silent)
		{
			if (greenHpAud == null)
			{
				greenHpAud = greenHpFlash.GetComponent<AudioSource>();
			}
			greenHpAud.Play();
			Object.Instantiate(scrnBlood, fullHud.transform);
		}
	}

	public void Respawn()
	{
		if (sliding)
		{
			StopSlide();
		}
		hp = 100;
		boostCharge = 299f;
		recentHealth = 0f;
		rb.constraints = defaultRBConstraints;
		activated = true;
		blackScreen.gameObject.SetActive(false);
		cc.enabled = true;
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		component.stylePoints = component.stylePoints / 3 * 2;
		if (gunc == null)
		{
			gunc = GetComponentInChildren<GunControl>();
		}
		gunc.YesWeapon();
		screenHud.SetActive(true);
		dead = false;
		blackColor.a = 0f;
		youDiedColor.a = 0f;
		currentAllPitch = 1f;
		blackScreen.color = blackColor;
		youDiedText.color = youDiedColor;
		audmix.SetFloat("allPitch", currentAllPitch);
		if (punch == null)
		{
			punch = GetComponentInChildren<Punch>();
		}
		punch.enabled = true;
		punch.transform.GetChild(0).gameObject.SetActive(true);
		punch.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
	}

	private void NotJumping()
	{
		jumping = false;
	}

	private void StartSlide()
	{
		if (currentSlideParticle != null)
		{
			Object.Destroy(currentSlideParticle);
		}
		if (slideScrape != null)
		{
			Object.Destroy(slideScrape);
		}
		playerCollider.height = 1.25f;
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.625f, base.transform.position.z);
		cc.defaultPos = new Vector3(cc.originalPos.x, cc.originalPos.y - 0.625f, cc.originalPos.z);
		slideSafety = 1f;
		sliding = true;
		boost = true;
		dodgeDirection = movementDirection;
		if (dodgeDirection == Vector3.zero)
		{
			dodgeDirection = base.transform.forward;
		}
		Quaternion identity = Quaternion.identity;
		identity.SetLookRotation(dodgeDirection * -1f);
		currentSlideParticle = Object.Instantiate(slideParticle, base.transform.position + dodgeDirection * 10f, identity);
		slideScrape = Object.Instantiate(slideScrapePrefab, base.transform.position + dodgeDirection * 2f, identity);
		if (dodgeDirection == base.transform.forward)
		{
			cc.dodgeDirection = 0;
		}
		else if (dodgeDirection == base.transform.forward * -1f)
		{
			cc.dodgeDirection = 1;
		}
		else
		{
			cc.dodgeDirection = 2;
		}
	}

	private void StopSlide()
	{
		if (currentSlideParticle != null)
		{
			Object.Destroy(currentSlideParticle);
		}
		else if (slideScrape != null)
		{
			Object.Destroy(slideScrape);
		}
		Object.Instantiate(slideStopSound);
		playerCollider.height = 2.5f;
		cc.defaultPos = cc.originalPos;
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.625f, base.transform.position.z);
		cc.ResetToDefaultPos();
		sliding = false;
		if (slideLength > longestSlide)
		{
			longestSlide = slideLength;
		}
		Debug.Log("Slide Length: " + slideLength);
		slideLength = 0f;
	}

	public void EmptyStamina()
	{
		boostCharge = 0f;
	}

	public void FullStamina()
	{
		boostCharge = 300f;
	}
}
